using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVCClient.Models;
using Newtonsoft.Json;


namespace MVCClient.Controllers
{
    public class LoggeadoController : Controller
    {
        static Usuario usuarioLoggeado;
        static Usuario usuarioReceptor;

        public ActionResult Index(string id)
        {
            if (id != string.Empty)
            {
                usuarioLoggeado = (Usuario)APIConnection.ObtenerUsuario($"Usuarios/GetUserByID/{id}", "objeto");
                APIConnection.Loggeado = usuarioLoggeado;
                return View("Index", new { id = usuarioLoggeado.ID });
            }
            else if (usuarioLoggeado != null)
            {
                return View("Index", new { id = usuarioLoggeado.ID });
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpGet]
        public ActionResult Conversacion(string id)
        {
            usuarioReceptor = (Usuario)APIConnection.ObtenerUsuario($"Usuarios/GetUserByID/{id}", "objeto");

            if (usuarioReceptor.Username != null)
            {
                Session["UsuarioReceptor"] = $"{usuarioReceptor.Username} {usuarioReceptor.Nombre}";
                Session["UsuarioReceptorID"] = usuarioReceptor.ID;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return RedirectToAction("Login", "Home");
            }

            var mensajes = (List<Conversaciones>)APIConnection.ObteniendoMensajes($"Conversaciones/GetMensajes/{usuarioLoggeado.Username}/{usuarioReceptor.Username}");
            var mensajesDescifrados = new List<Conversaciones>();

            if (mensajes.Count > 0)
            {
                foreach (var item in mensajes)
                {
                    if (item.Archivo == false)
                    {
                        var a = Convert.ToInt32(Convert.ToByte(item.UserEmisor[0]));
                        var b = Convert.ToInt32(Convert.ToByte(item.UserReceptor[0]));
                        var g = a + b;
                        var K = DiffieHellman.GenerandoLlave(a, b, g);
                        var Llave = Convert.ToString(K, 2).PadLeft(10, '0');
                        var direccionPermutaciones = Server.MapPath($"~/Permutaciones.txt");
                        var NSDES = new Cifrado();
                        var Permutaciones = NSDES.ObtenerPermutaciones(direccionPermutaciones);
                        NSDES.ObtenerKas(Llave, Permutaciones);

                        NSDES.S0[0, 0] = "01"; NSDES.S0[0, 1] = "00"; NSDES.S0[0, 2] = "11"; NSDES.S0[0, 3] = "10";
                        NSDES.S0[1, 0] = "11"; NSDES.S0[1, 1] = "10"; NSDES.S0[1, 2] = "01"; NSDES.S0[1, 3] = "00";
                        NSDES.S0[2, 0] = "00"; NSDES.S0[2, 1] = "10"; NSDES.S0[2, 2] = "01"; NSDES.S0[2, 3] = "11";
                        NSDES.S0[3, 0] = "11"; NSDES.S0[3, 1] = "01"; NSDES.S0[3, 2] = "11"; NSDES.S0[3, 3] = "10";

                        NSDES.S1[0, 0] = "00"; NSDES.S1[0, 1] = "01"; NSDES.S1[0, 2] = "10"; NSDES.S1[0, 3] = "11";
                        NSDES.S1[1, 0] = "10"; NSDES.S1[1, 1] = "00"; NSDES.S1[1, 2] = "01"; NSDES.S1[1, 3] = "11";
                        NSDES.S1[2, 0] = "11"; NSDES.S1[2, 1] = "00"; NSDES.S1[2, 2] = "01"; NSDES.S1[2, 3] = "00";
                        NSDES.S1[3, 0] = "10"; NSDES.S1[3, 1] = "01"; NSDES.S1[3, 2] = "00"; NSDES.S1[3, 3] = "11";

                        var mensajeBytes = Encoding.Default.GetBytes((string)item.Mensaje);
                        var txtDescifrado = new byte[mensajeBytes.Length];
                        var contador = 0;

                        foreach (var bait in mensajeBytes)
                        {
                            var ByteLeido = Convert.ToString(bait, 2); //Enviar ByteLeido a metodo para cifrar
                            while (ByteLeido.Length < 8)
                            {
                                ByteLeido = $"0{ByteLeido}";
                            }
                            var ByteCifrado = NSDES.DescifrarByte(ByteLeido, Permutaciones);
                            txtDescifrado[contador] = Convert.ToByte(ByteCifrado);
                            contador++;
                        }

                        item.Mensaje = Encoding.Default.GetString(txtDescifrado, 0, txtDescifrado.Count());

                        mensajesDescifrados.Add(item);
                    }
                    //Agregar cuando tenga archivo el else
                }
                return View(mensajesDescifrados);
            }
            return View(mensajes);
        }

        [HttpPost]
        public ActionResult Conversacion(string mensaje, HttpPostedFileBase archivo)
        {
            Usuario receptor = (Usuario)APIConnection.ObtenerUsuario($"Usuarios/GetUserByID/{Session["UsuarioReceptorID"]}", "objeto");

            if (receptor.Username != null)
            {
                Session["UsuarioReceptor"] = $"{usuarioReceptor.Username} {usuarioReceptor.Nombre}";
                Session["UsuarioReceptorID"] = usuarioReceptor.ID;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server eror. Please contact administrator.");
                return RedirectToAction("Login", "Home");
            }

            var mensajeNuevo = new Conversaciones();
            mensajeNuevo.UserEmisor = usuarioLoggeado.Username;
            mensajeNuevo.UserReceptor = receptor.Username;
            mensajeNuevo.Hora = DateTime.Now;

            if (mensaje == "" && archivo == null)
            {
                ViewBag.MensajeError = "Mensaje totalmente vacío, mandar algo";
                return RedirectToAction("Conversacion", new { id = receptor.ID });
            }
            else if (mensaje != "" && archivo == null)
            {
                var a = Convert.ToInt32(Convert.ToByte(mensajeNuevo.UserEmisor[0]));
                var b = Convert.ToInt32(Convert.ToByte(mensajeNuevo.UserReceptor[0]));
                var g = a + b;
                var K = DiffieHellman.GenerandoLlave(a, b, g);
                var Llave = Convert.ToString(K, 2).PadLeft(10, '0');
                var direccionPermutaciones = Server.MapPath($"~/Permutaciones.txt");
                var NSDES = new Cifrado();
                var Permutaciones = NSDES.ObtenerPermutaciones(direccionPermutaciones);
                NSDES.ObtenerKas(Llave, Permutaciones);

                NSDES.S0[0, 0] = "01"; NSDES.S0[0, 1] = "00"; NSDES.S0[0, 2] = "11"; NSDES.S0[0, 3] = "10";
                NSDES.S0[1, 0] = "11"; NSDES.S0[1, 1] = "10"; NSDES.S0[1, 2] = "01"; NSDES.S0[1, 3] = "00";
                NSDES.S0[2, 0] = "00"; NSDES.S0[2, 1] = "10"; NSDES.S0[2, 2] = "01"; NSDES.S0[2, 3] = "11";
                NSDES.S0[3, 0] = "11"; NSDES.S0[3, 1] = "01"; NSDES.S0[3, 2] = "11"; NSDES.S0[3, 3] = "10";

                NSDES.S1[0, 0] = "00"; NSDES.S1[0, 1] = "01"; NSDES.S1[0, 2] = "10"; NSDES.S1[0, 3] = "11";
                NSDES.S1[1, 0] = "10"; NSDES.S1[1, 1] = "00"; NSDES.S1[1, 2] = "01"; NSDES.S1[1, 3] = "11";
                NSDES.S1[2, 0] = "11"; NSDES.S1[2, 1] = "00"; NSDES.S1[2, 2] = "01"; NSDES.S1[2, 3] = "00";
                NSDES.S1[3, 0] = "10"; NSDES.S1[3, 1] = "01"; NSDES.S1[3, 2] = "00"; NSDES.S1[3, 3] = "11";

                var mensajeBytes = Encoding.Default.GetBytes((string)mensaje);
                var txtCifrado = new byte[mensajeBytes.Length];
                var contador = 0;

                foreach (var bait in mensajeBytes)
                {
                    var ByteLeido = Convert.ToString(bait, 2); //Enviar ByteLeido a metodo para cifrar
                    while (ByteLeido.Length < 8)
                    {
                        ByteLeido = $"0{ByteLeido}";
                    }
                    var ByteCifrado = NSDES.CifrarByte(ByteLeido, Permutaciones);
                    txtCifrado[contador] = Convert.ToByte(ByteCifrado);
                    contador++;
                }
                mensajeNuevo.Mensaje = Encoding.Default.GetString(txtCifrado);
            }
            // else if mensaje == "" && archivo != null

            var envioMensaje = APIConnection.EnviandoMensajes("Conversaciones", mensajeNuevo);

            if (envioMensaje)
            {
                return RedirectToAction("Conversacion", new { id = receptor.ID });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return RedirectToAction("Conversacion", new { id = receptor.ID });
            }
        }
    }
}

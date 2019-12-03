using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                        var K = DiffieHellman.GenerandoLlave(a, b, 9);

                        var txtDescifrado = Cifrado.DescifrarHorario(item.Mensaje, K);

                        item.Mensaje = txtDescifrado;

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
                var K = DiffieHellman.GenerandoLlave(a, b, 9);

                mensajeNuevo.Archivo = false;

                var txtCifrado = Cifrado.CifrarHorario(mensaje, K);

                mensajeNuevo.Mensaje = txtCifrado;
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

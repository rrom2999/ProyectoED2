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
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50026/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage responseTask = client.GetAsync($"Usuarios/GetUserByID/{id}").Result;

                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();

                    usuarioLoggeado = readTask.Result;
                }
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
            if (id != string.Empty)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50026/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage responseTask = client.GetAsync($"Usuarios/GetUserByID/{id}").Result; 

                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();

                    usuarioReceptor = readTask.Result;
                }
            }
            else
            {
                usuarioReceptor = null;
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            if (usuarioReceptor.Username != null)
            {
                Session["UsuarioReceptor"] = $"{usuarioReceptor.Username} {usuarioReceptor.Nombre}";
                Session["UsuarioReceptorID"] = usuarioReceptor.ID;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server eror. Please contact administrator.");
                return RedirectToAction("Login", "Home");
            }

            var mensajesCifrados = new List<Conversaciones>();
            var mensajesDescifrados = new List<Conversaciones>();

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:50026/");
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseTask = cliente.GetAsync($"Conversaciones/GetMensajes/{usuarioLoggeado.Username}/{usuarioReceptor.Username}").Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Content.ReadAsAsync<IList<Conversaciones>>();
                    readTask.Wait();

                    mensajesCifrados = (List<Conversaciones>)readTask.Result;
                }
                
            }

            if (mensajesCifrados.Count > 0)
            {
                foreach (var item in mensajesCifrados)
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
                    //Agregar cuando tenga archivo
                    return View(mensajesDescifrados);
                }
            }

            return View(mensajesCifrados);
        }

        [HttpPost]
        public ActionResult Conversacion(string mensaje, HttpPostedFileBase archivo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50026/");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseTask = client.GetAsync($"Usuarios/GetUserByID/{Session["UsuarioReceptorID"]}").Result;

                var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                readTask.Wait();

                usuarioReceptor = readTask.Result;
            }

            if (usuarioReceptor.Username != null)
            {
                Session["UsuarioReceptor"] = $"{usuarioReceptor.Username} {usuarioReceptor.Nombre}";
                Session["UsuarioReceptorID"] = usuarioReceptor.ID;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server eror. Please contact administrator.");
                return RedirectToAction("Login", "Home");
            }

            var nuevoMensaje = new Conversaciones();
            nuevoMensaje.UserEmisor = usuarioLoggeado.Username;
            nuevoMensaje.UserReceptor = usuarioReceptor.Username;
            nuevoMensaje.Hora = DateTime.Now;

            if (mensaje == string.Empty && archivo == null)
            {
                ViewBag.MensajeError = "Mensaje totalmente vacío, mandar algo";
                return RedirectToAction("Conversacion", new { id = usuarioReceptor.ID });
            }
            else if (mensaje != string.Empty && archivo == null)
            {
                var a = Convert.ToInt32(Convert.ToByte(nuevoMensaje.UserEmisor[0]));
                var b = Convert.ToInt32(Convert.ToByte(nuevoMensaje.UserReceptor[0]));
                var K = DiffieHellman.GenerandoLlave(a, b, 9);

                nuevoMensaje.Archivo = false;

                var txtCifrado = Cifrado.CifrarHorario(mensaje, K);

                nuevoMensaje.Mensaje = txtCifrado;
            }
            //agregar si esta vacio el mensaje pero si tiene un archivo

            bool envioMensaje;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50026/");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = client.PostAsJsonAsync($"Conversaciones", nuevoMensaje)) 
                {
                    if (response.Result.IsSuccessStatusCode)
                    {
                        envioMensaje = true;
                    }
                    else
                    {
                        envioMensaje = false;
                    }
                }
            }

            if (envioMensaje == true)
            {
                return RedirectToAction("Conversacion", new { id = usuarioReceptor.ID });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server eror. Please contact administrator.");
                return RedirectToAction("Conversacion", new { id = usuarioReceptor.ID });
            }
        }
    }
}

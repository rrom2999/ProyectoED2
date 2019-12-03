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
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string USERNAME, string PASSWORD)
        {
            Usuario Devuelto = new Usuario();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(APIConnection.LocalHostPort);

                var responseTask = httpClient.GetAsync($"Usuarios/GetUser/{USERNAME}").Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();

                    Devuelto = readTask.Result;
                }
                else
                {
                    Devuelto = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                string CC = Cifrado.CESAR(PASSWORD, "Adulterio");
                if (Devuelto != null && CC == Devuelto.Password)
                {
                    Session["LoggedName"] = Devuelto.Nombre;
                    Session["ApellidoLoggeado"] = Devuelto.Apellido;
                    return RedirectToAction("Index", "Loggeado",  new { id = Devuelto.ID });
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult CreacionUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreacionUser(string name, string lastname, string username, string password)
        {
            Usuario NuevoUser = new Usuario();
            NuevoUser.Nombre = name; NuevoUser.Apellido = lastname; NuevoUser.Username = username; NuevoUser.Password = Cifrado.CESAR(password, "Adulterio");

            Usuario Devuelto = new Usuario();

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(APIConnection.LocalHostPort);
                
                var responseTask = httpClient.GetAsync($"Usuarios/GetUser/{username}").Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();
                    Devuelto = readTask.Result;
                }
                else 
                {
                    Devuelto = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

                if(Devuelto == null)
                {
                    var response = httpClient.PostAsJsonAsync("Usuarios", NuevoUser).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.MensajeLogin = "Creacion usuario exitosa";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.MensajeCreacion = "Creacion de usuario fallida";
                        return View();
                    }
                }
                else
                {
                    ViewBag.MensajeCreacion = "El Usuario ya existe";
                    return View();
                }
            }
        }

        public ActionResult VerificacionUsuario(string name, string lastname, string username, string password, string id)
        {
            Usuario loggeado = new Usuario();
            loggeado.ID = id;
            IEnumerable<Usuario> users = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIConnection.LocalHostPort);
                //HTTP GET
                var responseTask = client.GetAsync("Usuarios");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    users = JsonConvert.DeserializeObject<IList<Usuario>>(readTask.Result);

                }
                else
                {
                    users = Enumerable.Empty<Usuario>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(users);
        }

        public ActionResult EliminacionUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EliminacionUser(string usernameE, string passwordE)
        {
            Usuario AEliminar = new Usuario(); AEliminar.Username = usernameE; AEliminar.Password = Cifrado.CESAR(passwordE, "Adulterio");
            Usuario Devuelto = new Usuario();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(APIConnection.LocalHostPort);

                var responseTask = httpClient.GetAsync($"Usuarios/GetUser/{usernameE}").Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();
                    Devuelto = readTask.Result;
                }
                else
                {
                    Devuelto = null; 
                    //Agregar ViewBag
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

                if (Devuelto != null)
                {
                    if(Devuelto.Password == AEliminar.Password)
                    {
                        var response = httpClient.DeleteAsync($"Usuarios/DeleteUser/{usernameE}").Result;
                        
                        if (response.IsSuccessStatusCode)
                        {
                            ViewBag.MensajeLogin = "Eliminacion realizada";
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.MensajeEliminacion = "Password incorrecto";
                        return View();
                    }
                }
                else
                {
                    ViewBag.MensajeEliminacion = "El Usuario no existe";
                    return View();
                }
            }

        }

        public ActionResult ModificacionUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ModificacionUser(string name, string lastname, string username, string password)
        {
            return View();
        }

        public ActionResult LlamandoReceptor(string id)
        {
            var userReceptor = new Usuario();
            userReceptor.ID = id;
            return RedirectToAction("Conversacion", "Loggeado", new { id = userReceptor.ID });
        }
    }
}
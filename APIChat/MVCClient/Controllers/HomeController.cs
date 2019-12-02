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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

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
                httpClient.BaseAddress = new Uri("http://localhost:57877/");

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
                    // RedirectToAction Nombre de Metodo para chat
                    return RedirectToAction("VerificacionUsuario");
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
                httpClient.BaseAddress = new Uri("http://localhost:57877/");
                
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
                        
                        return View();
                    }
                }
                else
                {
                    ViewBag.ResultadoCreacion = "El Usuario ya existe";
                    return View();
                }
            }
        }

        public ActionResult VerificacionUsuario(string name, string lastname, string username, string password)
        {
            Usuario aCrear = new Usuario();
            aCrear.Nombre = name; aCrear.Apellido = lastname; aCrear.Username = username; aCrear.Password = password;
            IEnumerable<Usuario> users = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:57877/");
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

        public ActionResult Conversacion(string user, string texto)
        {
            Conversaciones aMandar = new Conversaciones();
            var listaMen = new List<string>();
            listaMen.Add("Hola");
            listaMen.Add("Como estas?");
            listaMen.Add(texto);
            return View(listaMen);
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
                httpClient.BaseAddress = new Uri("http://localhost:57877/");

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
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                        //Agregar ViewBag
                    }
                }
                else
                {
                    ViewBag.ResultadoEliminacion = "El Usuario no existe";
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
    }
}
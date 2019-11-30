using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVCClient.Models;

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
                httpClient.BaseAddress = new Uri("http://localhost:50362/");

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
                    return RedirectToAction("CreacionUser");
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
            NuevoUser.Nombre = name; NuevoUser.Apellido = lastname; NuevoUser.Username = username; NuevoUser.Password = password;

            Usuario Devuelto = new Usuario();

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:50362/");
                
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
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        return View();
                    }
                }
                


            }
                return View();
        }

        public ActionResult VerificacionUsuario(string name, string lastname, string username, string password)
        {
            Usuario aCrear = new Usuario();
            aCrear.Nombre = name; aCrear.Apellido = lastname; aCrear.Username = username; aCrear.Password = password;
            var listaUsuarios = new List<string>();
            listaUsuarios.Add("Boris");
            listaUsuarios.Add("Ronald");
            return View(listaUsuarios);
        }

        public ActionResult Conversacion(string user, string texto)

        {
            var listaMen = new List<string>();
            listaMen.Add("Hola");
            listaMen.Add("Como estas?");
            listaMen.Add(texto);
            return View(listaMen);
        }
    }
}
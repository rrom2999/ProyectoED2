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
        public ActionResult Conversacion(string id)
        {
            var userReceptor = new Usuario();
            if (id != string.Empty)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:53010/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage responseTask = client.GetAsync($"Usuarios/GetUserByID/{id}").Result; 

                    var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                    readTask.Wait();

                    userReceptor = readTask.Result;

                    return View("Conversacion", new { loggeado = userReceptor });
                }
            }
            else if (userReceptor != null)
            {
                return View("Conversacion", new { loggeado = userReceptor });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}

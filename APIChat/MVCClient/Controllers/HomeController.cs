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

        public ActionResult CreacionUser()
        {
            return View();
        }

        public async Task<ActionResult> CreacionUser(string name, string lastname, string username, string password)
        {
            Usuario NuevoUser = new Usuario();
            NuevoUser.Nombre = name; NuevoUser.Apellido = lastname; NuevoUser.Username = username; NuevoUser.Password = password;

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:50064");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var respuesta = await httpClient.PostAsJsonAsync("Usuarios", NuevoUser))
                {

                }
            }
                return View();
        }
    }
}
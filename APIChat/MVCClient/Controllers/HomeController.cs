using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult VerificacionUsuario()
        {
            return View();
        }

        public ActionResult VerificacionUsuario(string name, string lastname, string username, string password)
        {
            Usuario aCrear = new Usuario();
            aCrear.Nombre = name; aCrear.Apellido = lastname; aCrear.Username = username; aCrear.Password = password;
            return View();
        }

        public ActionResult CreacionUser()
        {
            return View();
        }
    }
}
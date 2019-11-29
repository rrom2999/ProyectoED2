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

        public ActionResult CreacionUser()
        {
            return View();
        }
    }
}
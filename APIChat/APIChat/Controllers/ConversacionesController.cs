using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIChat.Models;
using APIChat.Services;


namespace APIChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversacionesController : ControllerBase
    {
        private readonly ConverCRUD conCRUD;

        public ConversacionesController(ConverCRUD CRUD)
        {
            conCRUD = CRUD;
        }

        [HttpGet]
        public IEnumerable<Conversaciones> Get()
        {
            var mensajes = conCRUD.Get();
            return mensajes;
        }

        [HttpGet, Route("GetUser/{userreceptor}")]
        public ActionResult<Conversaciones> Get(string userreceptor)
        {
            var conver = conCRUD.Get(userreceptor);

            return conver;
        }

        [HttpPost]
        public ActionResult<Conversaciones> Create(Conversaciones conver)
        {
            conCRUD.Create(conver);

            return CreatedAtRoute("GetBook", new { id = conver.ID.ToString() }, conver);
        }

        public IActionResult Update(string receptor, Conversaciones conver)
        {
            var conversacion = conCRUD.Get(receptor);

            if (conversacion == null)
            {
                return NotFound();
            }

            conCRUD.Update(receptor, conver);

            return NoContent();
        }

        public IActionResult Delete(string receptor)
        {
            var userR = conCRUD.Get(receptor);

            if (userR == null)
            {
                return NotFound();
            }

            conCRUD.Remove(receptor);

            return NoContent();
        }
    }
}

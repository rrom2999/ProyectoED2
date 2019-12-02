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
        public ActionResult<List<Conversaciones>> Get() =>
            conCRUD.GetTodo();

        [HttpGet, Route("GetMensajes/{emisor}/{receptor}")]
        public ActionResult<List<Conversaciones>> GetMensajes(string emisor, string receptor) =>
            conCRUD.GetMensajes(emisor, receptor);

        [HttpGet, Route("GetMensajeBuscado/{emisor}/{receptor}/{palabra}")]
        public ActionResult<List<Conversaciones>> GetMensajeBuscado(string emisor, string receptor, string palabra) =>
            conCRUD.GetMensajeBuscado(emisor, receptor, palabra);
        
        [HttpGet("{id:length(24)}", Name = "GetMensajes")]
        public ActionResult<Conversaciones> GetRegistro(string id, string user)
        {
            var mensaje = conCRUD.GetUnID(id);

            if (mensaje == null)
            {
                return NotFound();
            }
            return mensaje;
        }

        [HttpPost]
        public ActionResult<Conversaciones> Create(Conversaciones men)
        {
            conCRUD.Insertar(men);
            return CreatedAtRoute("GetMensajes", new { id = men.ID.ToString() }, men.Mensaje);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Conversaciones mensajeActualizado)
        {
            var mensaje = conCRUD.Get(id);

            if (mensaje == null)
            {
                return NotFound();
            }

            conCRUD.Update(id, mensajeActualizado);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var mensaje = conCRUD.Get(id);
            if (mensaje == null)
            {
                return NotFound();
            }

            conCRUD.DeleteOne(mensaje.ID);
            return NoContent();
        }

        [HttpPut("mensajes")]
        public IActionResult ActualizarEstadoDescarga(Conversaciones mensajeNuevo)
        {
            if (ModelState.IsValid)
            {
                conCRUD.ActualizarDesc(mensajeNuevo);
                return Ok();
            }
            return NotFound();
        }
    }
}

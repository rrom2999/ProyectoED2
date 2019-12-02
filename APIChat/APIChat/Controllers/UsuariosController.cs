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
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UserCRUD userCRUD;

        public UsuariosController(UserCRUD CRUD)
        {
            userCRUD = CRUD;
        }

        [HttpGet]
        public IEnumerable<Usuario> Get()
        {
            var users = userCRUD.Get();
            return users;
        }

        [HttpGet, Route("GetUser/{username}")]
        public ActionResult<Usuario> Get(string username)
        {
            var usuario = userCRUD.Get(username);

            return usuario;
        }
        [HttpPost]
        public ActionResult<Usuario> Create(Usuario usuario)
        {
            userCRUD.Create(usuario);

            return CreatedAtRoute("GetBook", new { id = usuario.ID.ToString() }, usuario);
        }

        public IActionResult Update(string username, Usuario usuarioIn)
        {
            var usuario = userCRUD.Get(username);

            if (usuario == null)
            {
                return NotFound();
            }

            userCRUD.Update(username, usuarioIn);

            return NoContent();
        }

        [Route("DeleteUser/{username}")]
        public IActionResult Delete(string username)
        {
            var usuario = userCRUD.Get(username);

            if (usuario == null)
            {
                return NotFound();
            }

            userCRUD.Remove(usuario);

            return NoContent();
        }
    }
}
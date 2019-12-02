using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIChat.Models;
using MongoDB.Driver;

namespace APIChat.Services
{
    public class UserCRUD
    {
        private readonly IMongoCollection<Usuario> UsuariosC;

        public UserCRUD(IBasedeDatosConfig settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            UsuariosC = database.GetCollection<Usuario>(settings.NombreColeccionUsuarios);
        }

        public List<Usuario> Get() =>
            UsuariosC.Find(Usuario => true).ToList();

        public Usuario Get(string username) =>
            UsuariosC.Find<Usuario>(usuario => usuario.Username == username).FirstOrDefault();

        public Usuario GetUserID(string id) =>
            UsuariosC.Find(usuario => usuario.ID == id).FirstOrDefault();


        public Usuario Create(Usuario usuario)
        {
            UsuariosC.InsertOne(usuario);
            return usuario;
        }

        public void Update(string username, Usuario usuarioIn) =>
            UsuariosC.ReplaceOne(usuario => usuario.Username == username, usuarioIn);

        public void Remove(Usuario usuarioIn) =>
            UsuariosC.DeleteOne(usuario => usuario.ID == usuarioIn.ID);

        public void Remove(string username) =>
            UsuariosC.DeleteOne(usuario => usuario.Username == username);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIChat.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace APIChat.Services
{
    public class ConverCRUD
    {
        private readonly IMongoCollection<Conversaciones> ConverC;

        public ConverCRUD(IBasedeDatosConfig setting)
        {
            var client = new MongoClient(setting.ConnectionString);
            var database = client.GetDatabase(setting.DatabaseName);

            ConverC = database.GetCollection<Conversaciones>(setting.NombreColeccionMensajes);
        }

        public void Insertar(Conversaciones nuevo)
        {
            ConverC.InsertOne(nuevo);
        }

        public List<Conversaciones> GetTodo()
        {
            return ConverC.Find(new BsonDocument()).ToList();
        }
        
        public Conversaciones GetUnID(string id)
        {
            return ConverC.Find(mensaje => mensaje.ID == id).FirstOrDefault();
        }

        public List<Conversaciones> GetMensajes(string emisor, string receptor)
        {
            var menEmisorReceptor = ConverC.Find(mensaje => mensaje.UserEmisor == emisor && mensaje.UserReceptor == receptor).ToList();
            var menReceptorEmisor = ConverC.Find(mensaje => mensaje.UserEmisor == receptor && mensaje.UserReceptor == emisor).ToList();

            return menEmisorReceptor.Concat(menReceptorEmisor).OrderBy(mens => mens.Hora).ToList();
        }

        public List<Conversaciones> GetMensajeBuscado(string emisor, string receptor, string palabra)
        {
            var menEmisorReceptor = ConverC.Find(mensaje => mensaje.UserEmisor == emisor && mensaje.UserReceptor == receptor).ToList();
            var menReceptorEmisor = ConverC.Find(mensaje => mensaje.UserEmisor == receptor && mensaje.UserReceptor == emisor).ToList();
            var menOrden = menEmisorReceptor.Concat(menReceptorEmisor).OrderBy(men => men.Hora).ToList();

            return menOrden;
        }

        public void Update(string id, Conversaciones mensajeActualizado) =>
            ConverC.ReplaceOne(men => men.UserReceptor == id, mensajeActualizado);

        public void DeleteOne(string id) =>
            ConverC.DeleteOne(men => men.ID == id);

        public void ActualizarDesc(Conversaciones mensajeNuevo)
        {
            ConverC.ReplaceOne(mensaje => mensaje.UserEmisor == mensajeNuevo.UserEmisor, mensajeNuevo);
        }

        public List<Conversaciones> Get()
        {
            return ConverC.Find(mensaje => true).SortBy(men => men.Hora).ToList();
        }

        public Conversaciones Get(string id) =>
            ConverC.Find(mens => mens.ID == id).FirstOrDefault();

        public Conversaciones GetMensaje(string mensaje)
        {
            return ConverC.Find(mens => mens.Mensaje.ToString() == mensaje).FirstOrDefault();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIChat.Models;
using MongoDB.Driver;

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

        public List<Conversaciones> Get() =>
            ConverC.Find(Emisor => true).ToList();

        public Conversaciones Get(string userReceptor) =>
            ConverC.Find<Conversaciones>(receptor => receptor.UserReceptor == userReceptor).FirstOrDefault();

        public Conversaciones Create(Conversaciones mensajes)
        {
            ConverC.InsertOne(mensajes);
            return mensajes;
        }

        public void Update(string receptor, Conversaciones mensajes) =>
            ConverC.ReplaceOne(re => re.UserReceptor == receptor, mensajes);

        public void Remove(Conversaciones mensajes) =>
            ConverC.DeleteOne(re => re.ID == mensajes.ID);

        public void Remove(string receptor) =>
            ConverC.DeleteOne(re => re.UserReceptor == receptor);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIChat.Models
{
    public class Conversaciones
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string UserEmisor { get; set; }
        public string UserReceptor { get; set; }
        public string Mensaje { get; set; }
        public DateTime Hora { get; set; }
        public bool Archivo { get; set; }
        public bool Descargado { get; set; }
    }
}
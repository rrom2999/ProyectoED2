using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Models
{
    public class Conversaciones
    {
        public string ID { get; set; }
        public string UserEmisor { get; set; }
        public string UserReceptor { get; set; }
        public string Mensaje { get; set; }
        public DateTime Hora { get; set; }
        public bool Archivo { get; set; }
    }
}
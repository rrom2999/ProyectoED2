using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCClient.Models
{
    public class Usuario
    {
        public string ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string  Password { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCClient.Models
{
    public class Mensaje
    {
        public string Texto { get; set; }

        public DateTime Date { get; set; }

        public Usuario Emisor { get; set; }

        public Usuario Receptor { get; set; }
    }
}
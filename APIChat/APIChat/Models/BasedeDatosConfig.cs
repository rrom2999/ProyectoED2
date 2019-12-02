using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat.Models
{
    public class BasedeDatosConfig : IBasedeDatosConfig
    {
        public string NombreColeccionUsuarios { get; set; }
        public string NombreColeccionMensajes { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBasedeDatosConfig
    {
        string NombreColeccionUsuarios { get; set; }
        string NombreColeccionMensajes { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}

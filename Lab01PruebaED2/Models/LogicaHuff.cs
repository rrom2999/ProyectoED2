using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lab01PruebaED2.Models;

namespace Lab01PruebaED2.Models
{
    public class LogicaHuff
    {
        public Dictionary<byte, Nodo> LlenarDicMaster(Dictionary<byte, Nodo> DicMaster, byte bLeido, byte[] bytes)
        {
            //acá declaro DMaster
            foreach (var item in bytes)
            {
                if (DicMaster.ContainsKey(bLeido) == true)
                {
                    DicMaster[bLeido].Frecuencia++;
                }
                else
                {
                    Nodo NuevoN = new Nodo();
                    NuevoN.Valor = bLeido;
                    NuevoN.Frecuencia = 1;
                    NuevoN.Derecho = null;
                    NuevoN.Izquierdo = null;
                    NuevoN.Padre = null;
                    DicMaster.Add(bLeido, NuevoN);
                }
            }
            return DicMaster;
        }

        public List<Nodo> OrdenarLDic (Dictionary<byte, Nodo> DMas)
        {
            Dictionary<byte, Nodo> AuxDMas = DMas;
            List<Nodo> DLista = new List<Nodo>();
            foreach (var item in AuxDMas)
            {
                Nodo Auxiliar = new Nodo();
                Auxiliar.Valor = item.Key;
                Auxiliar.Frecuencia = item.Value.Frecuencia;
                DLista.Add(Auxiliar);
            }
            IEnumerable<Nodo> ListadoOrdenado = DLista.OrderBy(x => x.Frecuencia);

            return ListadoOrdenado.ToList();
        }

        public void ArmarArbol (Nodo Izquierdo, Nodo Derecho, List<Nodo> Listado)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Lab01PruebaED2.Models;

namespace Lab01PruebaED2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download()
        {
            string path = Server.MapPath("~/ArchivoComprimido/");
            DirectoryInfo Info = new DirectoryInfo(path);
            FileInfo[] files = Info.GetFiles("*.*");
            List<string> Lista = new List<string>(files.Length);
            foreach(var item in files)
            {
                Lista.Add(item.Name);
            }
            return View(Lista);
        }

        public FileResult DownloadFile(string filename)
        {
            var Divisor = filename.Split('.');
            string fullpath = Path.Combine(Server.MapPath("~/ArchivoComprimido/"), filename);
            return File(fullpath, $"ArchivoCargado/{Divisor[1]}");
        }

        static int X = 0;
        public ActionResult CargarArchivo()
        {
            if (X > 0)
            {
                ViewBag.Msg = "Error al cargar el archivo";
            }
            X++;
            return View();
        }

        [HttpPost]
        public ActionResult CargarArchivo(HttpPostedFileBase AComprimir)
        {
            if (AComprimir != null)
            {
                LeerArchivo(AComprimir);
                return RedirectToAction("Download");
            }
            else
            {
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                return View();
            }
        }

        public void LeerArchivo(HttpPostedFileBase AComprimir)
        {

            Dictionary<byte, Nodo> DMaster = new Dictionary<byte, Nodo>();
            List<Nodo> ListadoOrden = new List<Nodo>();

            if (AComprimir != null && AComprimir.ContentLength > 0)
            {
                const int TBuffer = 1024;
                var NombreArchivo = AComprimir.FileName;
                var DireccionArchivo = Server.MapPath($"~/ArchivoCargado/{NombreArchivo}");
                var DireccionComprimido = Server.MapPath($"~/ArchivoComprimido");
                AComprimir.SaveAs(DireccionArchivo);

                using (var stream = new FileStream(DireccionArchivo, FileMode.Open))
                {
                    using (var Lector = new BinaryReader(stream))
                    {
                        var BytesBuffer = new byte[TBuffer];
                        while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                        {
                            BytesBuffer = Lector.ReadBytes(TBuffer);
                            foreach (var ByteLeido in BytesBuffer)
                            {
                                if (DMaster.ContainsKey(ByteLeido) == true)
                                {
                                    DMaster[ByteLeido].Frecuencia++;
                                }
                                else
                                {
                                    Nodo NuevoN = new Nodo();
                                    NuevoN.Valor = ByteLeido;
                                    NuevoN.Frecuencia = 1;
                                    NuevoN.Derecho = null;
                                    NuevoN.Izquierdo = null;
                                    NuevoN.Padre = null;
                                    DMaster.Add(ByteLeido, NuevoN);
                                }
                            }
                        }
                    }
                }
                PasarALista(DMaster, NombreArchivo, DireccionArchivo, DireccionComprimido);

            }

        }


        public void PasarALista(Dictionary<byte, Nodo> AOrdenar, string NombreArchivo, string RutaArchivo, string RutaComprimido)
        {
            Dictionary<byte, Nodo> AuxAOrdenar = AOrdenar;
            List<Nodo> DiccionarioEnlistado = new List<Nodo>();
            foreach (var ByteBase in AuxAOrdenar)
            {
                Nodo Auxiliar = new Nodo();
                Auxiliar.Valor = ByteBase.Key;
                Auxiliar.Frecuencia = ByteBase.Value.Frecuencia;
                DiccionarioEnlistado.Add(Auxiliar);
            }

            IEnumerable<Nodo> ListadoOrdenado = DiccionarioEnlistado.OrderBy(x => x.Frecuencia);
            ArmarArbol(ListadoOrdenado.ToList(), NombreArchivo, RutaArchivo, RutaComprimido);
        }

        public void ArmarArbol(List<Nodo> Listado, string NombreArchivo, string RutaArchivo, string RutaComprimido)
        {
            int NCaracteres = Listado.Count; //Para ver cuantas hojas se tienen
            Nodo Izquierdo = new Nodo();
            Nodo Derecho = new Nodo();
            while (Listado.Count != 1)
            {
                Nodo PadreNuevo = new Nodo();
                Izquierdo = Listado[0];
                Derecho = Listado[1];
                PadreNuevo.Izquierdo = Izquierdo;
                PadreNuevo.Derecho = Derecho;
                PadreNuevo.Frecuencia = Izquierdo.Frecuencia + Derecho.Frecuencia;
                Izquierdo.Padre = PadreNuevo; //Para heredar el prefijo
                Derecho.Padre = PadreNuevo;
                Listado.Remove(Izquierdo);
                Listado.Remove(Derecho);
                Listado.Add(PadreNuevo);
                IEnumerable<Nodo> FrecuenciasOrdenadas = Listado.OrderBy(x => x.Frecuencia);
                Listado = FrecuenciasOrdenadas.ToList();
            }

            Dictionary<byte, Nodo> DPrefijos = new Dictionary<byte, Nodo>();
            List<Nodo> PostOrdenList = new List<Nodo>();
            AsignarPrefijos(Listado[0]);

            PostOrden(PostOrdenList, Listado[0]);

            DPrefijos = LlenarDPrefijos(PostOrdenList, DPrefijos);
            EscribirComprimido(DPrefijos, NombreArchivo, RutaArchivo, RutaComprimido);
        }

        public List<Nodo> PostOrden(List<Nodo> ListPostOrden, Nodo nodoActual)
        {
            ListPostOrden.Clear();
            if (nodoActual != null)
            {
                PostOrden(nodoActual, ListPostOrden);
            }
            return ListPostOrden;
        }

        public void PostOrden(Nodo nodoRaiz, List<Nodo> ListPostOrden)
        {
            if (nodoRaiz != null)
            {
                PostOrden(nodoRaiz.Izquierdo, ListPostOrden);
                PostOrden(nodoRaiz.Derecho, ListPostOrden);
                if (nodoRaiz.Izquierdo == null && nodoRaiz.Derecho == null)
                {
                    ListPostOrden.Add(nodoRaiz);
                }
            }
        }

        public Dictionary<byte, Nodo> LlenarDPrefijos(List<Nodo> LPrefijos, Dictionary<byte, Nodo> DictionaryPrefijos)
        {
            List<Nodo> AuxLPrefijos = LPrefijos;
            Dictionary<byte, Nodo> AuxDictionaryPrefijos = DictionaryPrefijos;
            foreach (var nodo in AuxLPrefijos)
            {
                AuxDictionaryPrefijos.Add(nodo.Valor, nodo);
            }
            return AuxDictionaryPrefijos;
        }

        public void AsignarPrefijos(Nodo Raiz)
        {
            if (Raiz.Padre != null)
            {
                Raiz.Izquierdo.Prefijo = Raiz.Prefijo + "0";
                Raiz.Derecho.Prefijo = Raiz.Prefijo + "1";
            }
            else
            {
                Raiz.Izquierdo.Prefijo = "0";
                Raiz.Derecho.Prefijo = "1";
            }

            if (Raiz.Izquierdo.Izquierdo != null || Raiz.Izquierdo.Derecho != null)
            {
                AsignarPrefijos(Raiz.Izquierdo);
            }
            if (Raiz.Derecho.Izquierdo != null || Raiz.Derecho.Derecho != null)
            {
                AsignarPrefijos(Raiz.Derecho);
            }
        }

        public void EscribirComprimido(Dictionary<byte, Nodo> DPrefijos, string NombreArchivo, string RutaArchivo, string RutaComprimido)
        {
            var separado = NombreArchivo.Split('.');
            var Nombre = separado[0];
            var DireccionArchivo = RutaArchivo;
            var DireccionComprimido = RutaComprimido;
            var textBinLenght = 0;

            foreach (var caracter in DPrefijos)
            {
                textBinLenght = textBinLenght + (caracter.Value.Prefijo.Length * caracter.Value.Frecuencia);
            }

            using (var stream = new FileStream(DireccionArchivo, FileMode.Open))
            {
                using (var Lector = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream($"{DireccionComprimido}/{Nombre}.huff", FileMode.OpenOrCreate))
                    {
                        using (var Escritor = new BinaryWriter(writeStream))
                        {
                            var byteBuffer = new byte[2000];
                            var Texto = new List<char>();
                            var Prefijos = new List<char>();
                            var bytePrefijo = new List<byte>();
                            
                            while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                            {
                                byteBuffer = Lector.ReadBytes(2000);
                                var contador = 0;
                                foreach (var simbolo in byteBuffer)
                                {
                                    var codigoPrefijo = DPrefijos[simbolo].Prefijo.ToCharArray();
                                    foreach (var binario in codigoPrefijo)
                                    {
                                        contador++;
                                        Texto.Add(binario);

                                        var preLista = "";
                                        byte codigo;
                                        var valorDecimal = 0;


                                        if (Texto.Count >= 8)
                                        {
                                            foreach (var numeroBinario in Texto)
                                            {
                                                preLista = $"{preLista}{numeroBinario}";
                                            }
                                            valorDecimal = Convert.ToInt32(preLista, 2);
                                            codigo = (byte)valorDecimal;
                                            bytePrefijo.Add(codigo);
                                            Texto.Clear();
                                        }
                                        else if ((contador == textBinLenght) && (Texto.Count < 8))
                                        {
                                            foreach (var numeroBinario in Texto)
                                            {
                                                preLista = $"{preLista}{numeroBinario}";
                                            }
                                            valorDecimal = Convert.ToInt32(preLista, 2);
                                            codigo = (byte)valorDecimal;
                                            bytePrefijo.Add(codigo);
                                            Texto.Clear();
                                        }
                                    }
                                }
                            }
                            Escritor.Write(separado[1] + " ");
                            foreach (var codigoItem in DPrefijos)
                            {
                                Escritor.Write($"{codigoItem.Key};{codigoItem.Value.Prefijo};");
                            }
                            Escritor.Write("@@");

                            foreach (var caracter in bytePrefijo)
                            {
                                Escritor.Write(caracter);
                            }
                        }
                    }
                }
            }
            DownloadFile(NombreArchivo);
        }

        public ActionResult ErrorC()
        {
            return View();
        }

        public ActionResult ExitoC()
        {
            return View();
        }

    
        int Y = 0;
        public ActionResult CargaDescomprimir()
        {
            if(Y > 0)
            {
                ViewBag.Msg = "Error al cargar el archivo";

            }
            Y++;
            return View();
        }

        [HttpPost]
        public ActionResult CargaDescomprimir(HttpPostedFileBase HUFF)
        {
            if(HUFF != null)
            {
                LecturaComprimido(HUFF);
                return RedirectToAction("LecturaComprimido");
            }
            else
            {
                ViewBag.Msg = "Error al cargar Huff, intente de nuevo";
                return View();
            }
        }

        public void LecturaComprimido(HttpPostedFileBase HUFF)
        {
            var NombreComprimido = HUFF.FileName;
            var DireccioComprimido = Server.MapPath($"~/ArchivoComprimido/{NombreComprimido}");
            var RutaFinal = Server.MapPath($"~/ArchivoDescomprimido");
            Dictionary<string, string> DPrefijos = new Dictionary<string, string>();
            var PrimeraPosicion = true;
            var ExtensionArchivo = "";
            List<string> LecturaBytes = new List<string>();
            var byteprefijo = "";
            var NombreSeparado = NombreComprimido.Split('.');

            using (var stream = new FileStream(DireccioComprimido, FileMode.Open))
            {
                using (var Lector = new BinaryReader(stream))
                {
                    var byteLector = Lector.ReadString();
                    ExtensionArchivo = byteLector;
                }
            }
            using (var stream = new FileStream(DireccioComprimido, FileMode.Open))
            {
                using (var Lector = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream($"{RutaFinal}/{NombreSeparado[0]}.{ExtensionArchivo}", FileMode.OpenOrCreate))
                    {
                        using (var Escritor = new BinaryWriter(writeStream))
                        {
                            var Final = false;
                            var ListaBytes = new List<byte>();

                            while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                            {
                                //Llenando diccionario
                                if (Final != true)
                                {
                                    var byteLector = Lector.ReadString();
                                    if (byteLector == "@@")
                                    {
                                        Final = true;
                                    }
                                    else if (PrimeraPosicion == true)
                                    {
                                        ExtensionArchivo = byteLector;
                                        PrimeraPosicion = false;
                                    }
                                    else
                                    {
                                        var Conjunto = byteLector.Split(';');
                                        DPrefijos.Add(Conjunto[1], Conjunto[0]);
                                    }
                                }
                                else //Comienza a descomprimir
                                {
                                    var byteLector = Lector.ReadByte();
                                    ListaBytes.Add(byteLector);
                                }
                            }
                            var byteB = "";
                            var deci = 0;
                            char caracter;
                            var prefijo = "";

                            foreach (var item in ListaBytes)
                            {
                                byteB = byteB + Convert.ToString(item, 2).PadLeft(8, '0');
                            }
                            foreach (var item1 in byteB)
                            {
                                prefijo = prefijo + item1;
                                if (DPrefijos.ContainsKey(prefijo) == true)
                                {
                                    deci = Convert.ToInt32(DPrefijos[prefijo]);
                                    caracter = Convert.ToChar(deci);
                                    Escritor.Write(caracter);
                                    prefijo = "";
                                }
                            }
                        }
                    }
                    
                }
            }
            
        }
    }
}
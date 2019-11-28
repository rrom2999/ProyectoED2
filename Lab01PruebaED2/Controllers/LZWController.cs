using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Lab01PruebaED2.Controllers
{
    public class LZWController : Controller
    {
        // GET: LZW
        public ActionResult Index()
        {
            return View();
        }


        static int X = 0;
        public ActionResult CargarLZW()
        {
            if (X > 0)
            {
                ViewBag.Msg = "Error al cargar el archivo";
            }
            X++;
            return View();
        }



        [HttpPost]
        public ActionResult CargarLZW(HttpPostedFileBase AComprimirLZW)
        {
            if (AComprimirLZW != null)
            {
                LecturaLlenado(AComprimirLZW);
                return RedirectToAction("LeerArchivo");
            }
            else
            {
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                return View();
            }
        }

        const int TBuffer = 1024;

        public ActionResult LecturaLlenado(HttpPostedFileBase AComprimirLZW)
        {
            var Base = new Dictionary<string, int>();
            var BaseNuevasLlaves = new Dictionary<string, int>();
            var CodigosList = new List<int>();
            var anterior = "";
            var actual = "";
            var ContadorCodigo = 0;
            var ContadorLongitud = 0;
            if (AComprimirLZW != null && AComprimirLZW.ContentLength > 0)
            {

                var NombreArchivo = AComprimirLZW.FileName;
                var DireccionArchivo = Server.MapPath($"~/ArchivoCargado/{NombreArchivo}");
                var DireccionComprimido = Server.MapPath($"~/ArchivoComprimido");
                AComprimirLZW.SaveAs(DireccionArchivo);

                //Llenado de diccionario inicial                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          w                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
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
                                if (Base.ContainsKey(Convert.ToString(Convert.ToChar(ByteLeido))) == false)
                                {
                                    Base.Add(Convert.ToString(Convert.ToChar(ByteLeido)), ContadorCodigo);
                                    ContadorCodigo++;
                                }
                            }
                        }
                        foreach (var item in Base)
                        {
                            BaseNuevasLlaves.Add(item.Key, item.Value);
                        }
                        foreach (var item in BytesBuffer)
                        {
                            actual = Convert.ToString(Convert.ToChar(item));
                            ContadorLongitud++;
                            if (BaseNuevasLlaves.ContainsKey(anterior + actual) == true)
                            {
                                anterior += actual;
                            }
                            else
                            {
                                CodigosList.Add(BaseNuevasLlaves[anterior]);
                                BaseNuevasLlaves.Add(anterior + actual, ContadorCodigo);
                                ContadorCodigo++;
                                anterior = actual;
                                if (ContadorLongitud == Lector.BaseStream.Length)
                                {
                                    CodigosList.Add(BaseNuevasLlaves[anterior]);
                                }
                            }
                        }
                    }
                }

                EscribirComprimido(Base, NombreArchivo, DireccionArchivo, DireccionComprimido, CodigosList);
            }
            return RedirectToAction("DownloadFile");
        }

        public ActionResult DownloadFile()
        {
            var ubicacion = Server.MapPath($"~/ArchivoComprimido");
            var ubicacionD = Server.MapPath($"~/ArchivoDescomprimido");
            var directorio = new DirectoryInfo(ubicacion);
            var directorioD = new DirectoryInfo(ubicacionD);
            var filesC = directorio.GetFiles("*.*");
            var filesD = directorio.GetFiles("*.*");
            var listFiles = new List<string>(filesC.Length);
            var listFilesD = new List<string>(filesD.Length);
            foreach (var item in filesC)
            {
                listFiles.Add(item.Name);
            }
            foreach (var item in filesD)
            {
                listFiles.Add(item.Name);
            }
            return View("LecturaLlenado", listFiles);
        }

        public ActionResult Descargables(string fileName)

        {
            var name = fileName.Split('.');
            var DireccionCompleta = Path.Combine(Server.MapPath($"~//ArchivoComprimido"), fileName);
            return File(DireccionCompleta, "ArchivoComprimido", $"{name[0]}.{name[1]}");
            
        }

        public void EscribirComprimido(Dictionary<string, int> DicCodigos, string NombreArchivo, string RutaArchivo, string RutaComprimido, List<int> Codigos)
        {
            var separado = NombreArchivo.Split('.');
            var Nombre = separado[0];
            var DireccionArchivo = RutaArchivo;
            var DireccionComprimido = RutaComprimido;
            var byteCodigos = new List<byte>();
            var contBytes = 0;
            using (var stream = new FileStream(DireccionArchivo, FileMode.Open))
            {
                using (var writeStream = new FileStream($"{DireccionComprimido}/{Nombre}.lzw", FileMode.OpenOrCreate))
                {
                    using (var Escritor = new BinaryWriter(writeStream))
                    {
                        Escritor.Write(separado[1] + " ");
                        foreach (var item in DicCodigos)
                        {
                            Escritor.Write($"{item.Key}|{item.Value}|");
                        }
                        Escritor.Write("@@");
                        foreach (var item in Codigos)
                        {
                            if (item <= 255)
                            {
                                var ByteAux = Convert.ToByte(item);
                                contBytes = 0;
                                Escritor.Write(Convert.ToByte(contBytes));
                                Escritor.Write(ByteAux);
                            }
                            else
                            {
                                int auxiliar = item;
                                while (auxiliar > 255)
                                {
                                    auxiliar = auxiliar - 255;
                                    contBytes++;
                                }
                                Escritor.Write(Convert.ToByte(contBytes));
                                Escritor.Write(Convert.ToByte(auxiliar));
                            }
                        }
                    }
                }
            }
        }

        static int Y = 0;
        public ActionResult CargarDescompresionLZW()
        {
            if (Y > 0)
            {
                ViewBag.Msg = "Error al cargar el archivo";
            }
            Y++;
            return View();
        }

        [HttpPost]
        public ActionResult CargarDescompresionLZW(HttpPostedFileBase ADescomprimirLZW)
        {
            if (ADescomprimirLZW != null)
            {
                LecturaLlenado(ADescomprimirLZW);
                return RedirectToAction("DescompresionLZW");
            }
            else
            {
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                return View();
            }
        }

        public void LecturaComprimidoLZW(HttpPostedFileBase ADescomprimirLZW)
        {
            var NombreComprimido = ADescomprimirLZW.FileName;
            var NombreSeparado = NombreComprimido.Split('.');
            var DireccionComprimido = Server.MapPath($"~/ArchivoComprimido/{NombreComprimido}.txt");
            var RutaFinal = Server.MapPath($"~/ArchivoDescomprimido");
            Dictionary<int, string> DBase = new Dictionary<int, string>();
            var ExtensionArchivo = "";
            var FinalDiccionario = false;
            var PrimeraPosicion = true;
            var Caracter = "";
            var Primeravez = true;
            var ContadorBytes = new int();
            var PosicionV = new int();
            var PosicionN = new int();
            var Cadena = "";
            //Llenado de diccionario
            using (var stream = new FileStream(DireccionComprimido, FileMode.Open))
            {
                using (var Lector = new BinaryReader(stream))
                {
                    var byteLector = Lector.ReadString();
                    ExtensionArchivo = byteLector;
                }
            }
            using (var stream = new FileStream(DireccionComprimido, FileMode.Open))
            {
                using (var Lector = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream($"{RutaFinal}/{NombreSeparado[0]}.{ExtensionArchivo}", FileMode.OpenOrCreate))
                    {
                        using (var Escritor = new BinaryWriter(writeStream))
                        {

                            while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                            {
                                if (FinalDiccionario != true)
                                {
                                    var byteLector = Lector.ReadString();
                                    if (byteLector == "@@")
                                    {
                                        FinalDiccionario = true;
                                    }
                                    else if (PrimeraPosicion == true)
                                    {
                                        ExtensionArchivo = byteLector;
                                        PrimeraPosicion = false;
                                    }
                                    else
                                    {
                                        var Conjunto = byteLector.Split('|');
                                        DBase.Add(Convert.ToInt32(Conjunto[1]), Conjunto[0].ToString());
                                    }
                                }
                                else
                                {

                                    var byteLector = Lector.ReadByte();
                                    if (Primeravez)
                                    {
                                        //Leyendo Byte Viejo (Posicion)
                                        //Leer Contador de bytes "Llenos" para posicion
                                        ContadorBytes = Convert.ToInt32(byteLector);
                                        PosicionV = ContadorBytes * 255;
                                        byteLector = Lector.ReadByte();
                                        PosicionV = PosicionV + Convert.ToInt32(byteLector);
                                        //Se llena Caracter y se escribe
                                        Caracter = DBase[PosicionV];
                                        Escritor.Write(Caracter);
                                        Primeravez = false;
                                    }
                                    else
                                    {
                                        //Leer Byte Nuevo (Posicion)
                                        ContadorBytes = Convert.ToInt32(byteLector);
                                        PosicionN = ContadorBytes * 255;
                                        byteLector = Lector.ReadByte();
                                        PosicionN = PosicionN + Convert.ToInt32(byteLector);
                                        //if
                                        if (DBase.ContainsKey(PosicionN))
                                        {
                                            Cadena = DBase[PosicionN];
                                        }
                                        else
                                        {
                                            Cadena = DBase[PosicionV];
                                            Cadena += Caracter;
                                        }
                                        Escritor.Write(Cadena);
                                        Caracter = Convert.ToString(Cadena[0]);
                                        var ElementoNuevo = DBase[PosicionV] + Caracter;
                                        var UltimaPosicion = DBase.Count();
                                        DBase.Add(UltimaPosicion, ElementoNuevo);
                                        PosicionV = PosicionN;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
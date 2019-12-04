using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVCClient.Models
{
    public class Cifrado
    {
        public static string CESAR(string ACifrarC, string Clave)
        {
            var Repetida = false;
            Dictionary<string, string> Revisor = new Dictionary<string, string>();
            Dictionary<string, string> DMaster = new Dictionary<string, string>();
            string[] ABC = new string[54];
            ABC[0] = "A"; ABC[1] = "a"; ABC[2] = "B"; ABC[3] = "b"; ABC[4] = "C"; ABC[5] = "c"; ABC[6] = "D";
            ABC[7] = "d"; ABC[8] = "E"; ABC[9] = "e"; ABC[10] = "F"; ABC[11] = "f"; ABC[12] = "G"; ABC[13] = "g";
            ABC[14] = "H"; ABC[15] = "h"; ABC[16] = "I"; ABC[17] = "i"; ABC[18] = "J"; ABC[19] = "j"; ABC[20] = "K";
            ABC[21] = "k"; ABC[22] = "L"; ABC[23] = "l"; ABC[24] = "M"; ABC[25] = "m"; ABC[26] = "N";
            ABC[27] = "n"; ABC[28] = "Ñ"; ABC[29] = "ñ"; ABC[30] = "O"; ABC[31] = "o"; ABC[32] = "P"; ABC[33] = "p";
            ABC[34] = "Q"; ABC[35] = "q"; ABC[36] = "R"; ABC[37] = "r"; ABC[38] = "S"; ABC[39] = "s"; ABC[40] = "T";
            ABC[41] = "t"; ABC[42] = "U"; ABC[43] = "u"; ABC[44] = "V"; ABC[45] = "v"; ABC[46] = "W"; ABC[47] = "w";
            ABC[48] = "X"; ABC[49] = "x"; ABC[50] = "Y"; ABC[51] = "y"; ABC[52] = "Z"; ABC[53] = "z";

            for (int i = 0; i < Clave.Length; i++)
            {
                if (Revisor.ContainsKey(Clave[i].ToString()) == false)
                {
                    Revisor.Add(Clave[i].ToString(), Clave[i].ToString());
                }
                else
                {
                    Repetida = true;
                }
            }
            if (Repetida == false)
            {
                var Ultima = Clave.Length;
                for (int i = 0; i < Clave.Length; i++)
                {
                    DMaster.Add(ABC[i], Convert.ToString(Clave[i]));
                }
                for (int j = 0; j < 54; j++)
                {
                    if (DMaster.ContainsValue(ABC[j]) == false)
                    {
                        DMaster.Add(ABC[Ultima], ABC[j]);
                        Ultima++;
                    }
                }
            }

            string Cifrado = "";
            foreach (var Caracter in ACifrarC)
            {
                var CaracterString = Convert.ToString(Convert.ToChar(Caracter));
                if (DMaster.ContainsKey(CaracterString))
                {
                    Cifrado += DMaster[CaracterString];
                }
                else
                {
                    Cifrado += CaracterString;
                }
            }
            return Cifrado;
        }

        public string K1;
        public string K2;
        public string[,] S0 = new string[4, 4];
        public string[,] S1 = new string[4, 4];

        public string[] ObtenerPermutaciones(string Ruta)
        {
            string[] permutaciones = new string[5];
            var cadena = string.Empty;
            using (var stream = new FileStream(Ruta, FileMode.Open))
            {
                using (var Lector = new StreamReader(stream))
                {
                    while (!Lector.EndOfStream)
                    {
                        cadena = Lector.ReadLine();
                    }
                }
            }
            permutaciones = cadena.Split('@');
            return permutaciones;
        }

        public string P10(string Binario, string Secuencia)
        {
            string Nuevo = "";
            if (Binario.Length < 10) Binario.PadLeft(10, '0');

            for (int i = 0; i < 10; i++)
            {
                int Posicion = Convert.ToInt32(Convert.ToString(Secuencia[i]));
                Nuevo = $"{Nuevo}{Binario[Posicion]}";
            }
            return Nuevo;
        }

        public string P8(string Binario, string Secuencia)
        {
            var Nuevo = "";

            for (int i = 0; i < 8; i++)
            {
                int Posicion = Convert.ToInt32(Convert.ToString(Secuencia[i]));
                Nuevo = $"{Nuevo}{Binario[Posicion]}";
            }

            return Nuevo;
        }

        public string P4(string Binario, string Secuencia)
        {
            var Nuevo = "";

            for (int i = 0; i < 4; i++)
            {
                var Posicion = Convert.ToInt32(Convert.ToString(Secuencia[i]));
                Nuevo = $"{Nuevo}{Binario[Posicion]}";
            }

            return Nuevo;
        }

        public string EP(string Binario, string Secuencia)
        {
            var Nuevo = "";

            for (int i = 0; i < 8; i++)
            {
                var Posicion = Convert.ToInt32(Convert.ToString(Secuencia[i]));
                Nuevo = $"{Nuevo}{Binario[Posicion]}";
            }

            return Nuevo;
        }

        public string IP(string Binario, bool Inverso, string Secuencia)
        {
            var Nuevo = "";
            if (!Inverso)
            {
                for (int i = 0; i < 8; i++)
                {
                    var Posicion = Convert.ToInt32(Convert.ToString(Secuencia[i]));
                    Nuevo = $"{Nuevo}{Binario[Posicion]}";
                }
            }
            else
            {
                var SecuenciaInversa = "";
                string[] VectorInverso = new string[8];
                for (int i = 0; i < 8; i++)
                {
                    VectorInverso[Convert.ToInt32(Convert.ToString(Secuencia[i]))] = Convert.ToString(i);
                }

                foreach (var Valor in VectorInverso)
                {
                    SecuenciaInversa = $"{SecuenciaInversa}{Valor}";
                }

                for (int i = 0; i < 8; i++)
                {
                    var Posicion = Convert.ToInt32(Convert.ToString(SecuenciaInversa[i]));
                    Nuevo = $"{Nuevo}{Binario[Posicion]}";
                }
            }

            return Nuevo;
        }

        public string XOR(string Uno, string Dos)
        {
            var ResultadoXOR = "";
            for (int i = 0; i < Uno.Length; i++)
            {
                if (Uno[i] != Dos[i]) ResultadoXOR += "1";
                else ResultadoXOR += "0";
            }

            return ResultadoXOR;
        }

        public void ObtenerKas(string Llave, string[] Secuencia)
        {
            var Binario = P10(Llave, Secuencia[0]);
            var ParteA = Binario.Substring(0, 5);
            var ParteB = Binario.Substring(5, 5);

            var ShifteadoUno = "";
            var Temporal = "";
            for (int a = 1; a < 5; a++)
            {
                Temporal = $"{Temporal}{ParteA[a]}";
            }
            ShifteadoUno = $"{Temporal}{ParteA[0]}"; //Primeros 5 con LS1
            Temporal = "";
            for (int b = 1; b < 5; b++)
            {
                Temporal = $"{Temporal}{ParteB[b]}";
            }
            ShifteadoUno = $"{ShifteadoUno}{Temporal}{ParteB[0]}";
            K1 = P8(ShifteadoUno, Secuencia[1]);

            //Volver a hacer para K2 sobre ShifteadoUno
            ParteA = ShifteadoUno.Substring(0, 5);
            ParteB = ShifteadoUno.Substring(5, 5);

            var ShifteadoDos = "";
            Temporal = "";
            for (int a = 2; a < 5; a++)
            {
                Temporal = $"{Temporal}{ParteA[a]}";
            }
            ShifteadoDos = $"{Temporal}{ParteA[0]}{ParteA[1]}"; //Primeros 5 con LS2 
            Temporal = "";
            for (int b = 2; b < 5; b++)
            {
                Temporal = $"{Temporal}{ParteB[b]}";
            }
            ShifteadoDos = $"{ShifteadoDos}{Temporal}{ParteB[0]}{ParteB[1]}";
            K2 = P8(ShifteadoDos, Secuencia[1]);
        }

        public string ObtenerDeSBoxes(string Binario)
        {
            var AuxFila = $"{Binario[3]}{Binario[0]}"; var AuxCol = $"{Binario[2]}{Binario[1]}"; //Al reves para facilitar la conversion a int
            var FilaS0 = 0; var ColS0 = 0;
            for (int i = 0; i < 2; i++)
            {
                if (AuxFila[i] == '1')
                    FilaS0 += (int)Math.Pow(2, i);

                if (AuxCol[i] == '1')
                    ColS0 += (int)Math.Pow(2, i);
            }

            AuxFila = $"{Binario[7]}{Binario[4]}"; AuxCol = $"{Binario[6]}{Binario[5]}";  //Al reves para facilitar conversion a int
            var FilaS1 = 0; var ColS1 = 0;
            for (int i = 0; i < 2; i++)
            {
                if (AuxFila[i] == '1')
                    FilaS1 += (int)Math.Pow(2, i);

                if (AuxCol[i] == '1')
                    ColS1 += (int)Math.Pow(2, i);
            }

            var ResultadoSBoxes = $"{S0[FilaS0, ColS0]}{S1[FilaS1, ColS1]}";
            return ResultadoSBoxes;
        }

        public int CifrarByte(string ByteLeido, string[] Secuencia)
        {
            var Binario = IP(ByteLeido, false, Secuencia[4]); //Paso1

            var ParteA = Binario.Substring(0, 4);
            var ParteB = Binario.Substring(4, 4);

            var NuevaParteB = EP(ParteB, Secuencia[3]); //Paso2
            NuevaParteB = XOR(NuevaParteB, K1); //Paso3

            var ResultadoSBoxes1 = ObtenerDeSBoxes(NuevaParteB); //Paso4
            ResultadoSBoxes1 = P4(ResultadoSBoxes1, Secuencia[2]); //Paso5
            var ParteC = XOR(ResultadoSBoxes1, ParteA); //Paso6

            var NuevoBinario = $"{ParteB}{ParteC}";//Paso7 y Paso8

            var ParteD = NuevoBinario.Substring(0, 4);
            var ParteE = NuevoBinario.Substring(4, 4);

            var NuevaParteE = EP(ParteE, Secuencia[3]); //Paso9
            NuevaParteE = XOR(NuevaParteE, K2); //Paso10

            var ResultadoSBoxes2 = ObtenerDeSBoxes(NuevaParteE); //Paso11
            ResultadoSBoxes2 = P4(ResultadoSBoxes2, Secuencia[2]); //Paso12
            var ParteF = XOR(ResultadoSBoxes2, ParteD); //Paso13

            var Nuevo = $"{ParteF}{ParteE}"; //Paso14
            Nuevo = IP(Nuevo, true, Secuencia[4]); //Paso15
            char[] Arreglo = Nuevo.ToCharArray();
            Array.Reverse(Arreglo);
            var NuevoInt = 0;

            for (int i = 0; i < Arreglo.Length; i++)
            {
                if (Arreglo[i] == '1') NuevoInt += (int)Math.Pow(2, i);
            }
            return NuevoInt;
        }

        public int DescifrarByte(string ByteLeido, string[] Secuencia)
        {
            var Binario = IP(ByteLeido, false, Secuencia[4]); //Paso1

            var ParteA = Binario.Substring(0, 4);
            var ParteB = Binario.Substring(4, 4);

            var NuevaParteB = EP(ParteB, Secuencia[3]); //Paso2
            NuevaParteB = XOR(NuevaParteB, K2); //Paso3

            var ResultadoSBoxes1 = ObtenerDeSBoxes(NuevaParteB); //Paso4
            ResultadoSBoxes1 = P4(ResultadoSBoxes1, Secuencia[2]); //Paso5
            var ParteC = XOR(ResultadoSBoxes1, ParteA); //Paso6

            var NuevoBinario = $"{ParteB}{ParteC}";//Paso7 y Paso8

            var ParteD = NuevoBinario.Substring(0, 4);
            var ParteE = NuevoBinario.Substring(4, 4);

            var NuevaParteE = EP(ParteE, Secuencia[3]); //Paso9
            NuevaParteE = XOR(NuevaParteE, K1); //Paso10

            var ResultadoSBoxes2 = ObtenerDeSBoxes(NuevaParteE); //Paso11
            ResultadoSBoxes2 = P4(ResultadoSBoxes2, Secuencia[2]); //Paso12
            var ParteF = XOR(ResultadoSBoxes2, ParteD); //Paso13

            var Nuevo = $"{ParteF}{ParteE}"; //Paso14
            Nuevo = IP(Nuevo, true, Secuencia[4]); //Paso15
            char[] Arreglo = Nuevo.ToCharArray();
            Array.Reverse(Arreglo);
            var NuevoInt = 0;

            for (int i = 0; i < Arreglo.Length; i++)
            {
                if (Arreglo[i] == '1') NuevoInt += (int)Math.Pow(2, i);
            }
            return NuevoInt;
        }
    }
}
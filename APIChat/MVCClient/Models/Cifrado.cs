using System;
using System.Collections.Generic;
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

        public static int CalculaM(int n, int Longitud)
        {
            var m = 0;
            if ((Longitud % n) == 0)
            {
                m = Longitud / n;
            }
            else
            {
                m = (Longitud / n) + 1;
            }
            return m;
        }

        public static string CompletarTexto(string texto, int m, int n)
        {
            var area = m * n;
            while (texto.Length < area)
            {
                texto = texto + "$";
            }
            return texto;
        }

        //Para cifrar
        public static void LlenarMatrizAbajo(string texto, char[,] matriz, int n, int m)
        {
            var contador = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matriz[j, i] = texto[contador];
                    contador++;
                }
            }
        }

        public static string LeerEspiralHorario(int m, int n, char[,] matriz)
        {
            int i, filaAux = 0, colAux = 0;
            var textoCifrado = "";

            while (filaAux < m && colAux < n)
            {
                for (i = colAux; i < n; i++)
                {
                    textoCifrado = textoCifrado + matriz[filaAux, i];
                }
                filaAux++;


                for (i = filaAux; i < m; i++)
                {
                    textoCifrado = textoCifrado + matriz[i, n - 1];
                }
                n--;


                if (filaAux < m)
                {
                    for (i = n - 1; i >= colAux; i--)
                    {
                        textoCifrado = textoCifrado + matriz[m - 1, i];
                    }
                    m--;
                }


                if (colAux < n)
                {
                    for (i = m - 1; i >= filaAux; i--)
                    {
                        textoCifrado = textoCifrado + matriz[i, colAux];
                    }
                    colAux++;
                }
            }
            return textoCifrado;
        }

        public static void LlenarMatrizAlLado(string texto, char[,] matriz, int n, int m)
        {
            var contador = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matriz[i, j] = texto[contador];
                    contador++;
                }
            }
        }

        public static string LeerEspiralAntiHorario(int m, int n, char[,] matriz)
        {
            int i, filaAux = 0, colAux = 0;
            var textoCifrado = "";

            while (filaAux < m && colAux < n)
            {
                for (i = filaAux; i < m; i++)
                {
                    textoCifrado = textoCifrado + matriz[i, colAux];
                }
                colAux++;

                for (i = colAux; i < n; i++)
                {
                    textoCifrado = textoCifrado + matriz[m - 1, i];
                }
                m--;

                if (colAux < n)
                {
                    for (i = m - 1; i >= filaAux; i--)
                    {
                        textoCifrado = textoCifrado + matriz[i, n - 1];
                    }
                    n--;
                }

                if (filaAux < m)
                {
                    for (i = n - 1; i >= colAux; i--)
                    {
                        textoCifrado = textoCifrado + matriz[filaAux, i];
                    }
                    filaAux++;
                }
            }
            return textoCifrado;
        }

        //Para descifrar
        public static char[,] AgregarEnEspiralHorario(int m, int n, string textoCifrado)
        {
            int i, filaAux = 0, colAux = 0, contador = 0; ;
            var matrizDescifrado = new char[m, n];

            while (filaAux < m && colAux < n)
            {
                for (i = colAux; i < n; i++)
                {
                    matrizDescifrado[filaAux, i] = textoCifrado[contador];
                    contador++;
                }
                filaAux++;


                for (i = filaAux; i < m; i++)
                {
                    matrizDescifrado[i, n - 1] = textoCifrado[contador];
                    contador++;
                }
                n--;


                if (filaAux < m)
                {
                    for (i = n - 1; i >= colAux; i--)
                    {
                        matrizDescifrado[m - 1, i] = textoCifrado[contador];
                        contador++;
                    }
                    m--;
                }


                if (colAux < n)
                {
                    for (i = m - 1; i >= filaAux; i--)
                    {
                        matrizDescifrado[i, colAux] = textoCifrado[contador];
                        contador++;
                    }
                    colAux++;
                }
            }
            return matrizDescifrado;
        }

        public static string LeerMatrizAbajo(char[,] matrizCifrada, int n, int m)
        {
            var textoDescifrado = "";
            var txt = "";

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    txt = txt + matrizCifrada[j, i];
                }
            }
            foreach (var item in txt)
            {
                if (item != '$')
                {
                    textoDescifrado = textoDescifrado + item;
                }
            }
            return textoDescifrado;
        }

        public static char[,] AgregarEnEspiralAntiHorario(int m, int n, string textoCifrado)
        {
            int i, filaAux = 0, colAux = 0, contador = 0; ;
            var matrizDescifrado = new char[m, n];

            while (filaAux < m && colAux < n)
            {
                for (i = filaAux; i < m; i++)
                {
                    matrizDescifrado[i, colAux] = textoCifrado[contador];
                    contador++;
                }
                colAux++;

                for (i = colAux; i < n; i++)
                {
                    matrizDescifrado[m - 1, i] = textoCifrado[contador];
                    contador++;
                }
                m--;

                if (colAux < n)
                {
                    for (i = m - 1; i >= filaAux; i--)
                    {
                        matrizDescifrado[i, n - 1] = textoCifrado[contador];
                        contador++;
                    }
                    n--;
                }

                if (filaAux < m)
                {
                    for (i = n - 1; i >= colAux; i--)
                    {
                        matrizDescifrado[filaAux, i] = textoCifrado[contador];
                        contador++;
                    }
                    filaAux++;
                }
            }
            return matrizDescifrado;
        }

        public static string LeerMatrizAlLado(char[,] matrizCifrada, int n, int m)
        {
            var textoDescifrado = "";
            var txt = "";

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    txt = txt + matrizCifrada[i, j];
                }
            }
            foreach (var item in txt)
            {
                if (item != '$')
                {
                    textoDescifrado = textoDescifrado + item;
                }
            }

            return textoDescifrado;
        }

        public static string CifrarHorario(string texto, int n)
        {
            var m = 0;
            m = CalculaM(n, texto.Length);
            var matrizCifrado = new char[m, n];
            var txtCif = "";
            texto = CompletarTexto(texto, m, n);

            LlenarMatrizAbajo(texto, matrizCifrado, n, m);
            txtCif = LeerEspiralHorario(m, n, matrizCifrado);

            return txtCif;
        }

        public static string DescifrarHorario(string textoCif, int n)
        {
            var m = 0;
            m = CalculaM(n, textoCif.Length);
            var matrizDescifrado = new char[m, n];
            var txtDesCif = "";

            textoCif = CompletarTexto(textoCif, m, n);
            matrizDescifrado = AgregarEnEspiralAntiHorario(n, m, textoCif);
            txtDesCif = LeerMatrizAlLado(matrizDescifrado, m, n);

            return txtDesCif;
        }

        public static string CifrarAntiHorario(string texto, int n)
        {
            var m = 0;
            m = CalculaM(n, texto.Length);
            var matrizCifrado = new char[m, n];
            var txtCif = "";

            texto = CompletarTexto(texto, m, n);
            LlenarMatrizAlLado(texto, matrizCifrado, n, m);
            txtCif = LeerEspiralAntiHorario(m, n, matrizCifrado);

            return txtCif;
        }

        public static string DescifrarAntiHorario(string textoCif, int n)
        {
            var m = 0;
            m = CalculaM(n, textoCif.Length);
            var matrizDescifrado = new char[m, n];
            var txtDesCif = "";

            textoCif = CompletarTexto(textoCif, m, n);
            matrizDescifrado = AgregarEnEspiralHorario(n, m, textoCif);
            txtDesCif = LeerMatrizAbajo(matrizDescifrado, m, n);

            return txtDesCif;
        }
    }
}
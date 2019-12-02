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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCClient.Models
{
    public class DiffieHellman
    {
        static int P = 23;

        public static int MOD(int bas, int exponente, int mod)
        {
            int residuo = 1;

            for (int i = 0; i < exponente; i++)
            {
                residuo = residuo * bas % mod;
            }

            return residuo;
        }

        public static int GenerandoLlave(int a, int b, int g)
        {
            var B = MOD(g, b, P);
            var K = MOD(B, a, P);

            return K;
        }
    }
}
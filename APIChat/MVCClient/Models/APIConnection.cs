using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MVCClient.Models
{
    public class APIConnection
    {
        public static string LocalHostPort = "http://localhost:50026/";
        public static Usuario Loggeado = new Usuario();

        public static object ObtenerUsuario(string controlador, string tipo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalHostPort);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseTask = client.GetAsync(controlador).Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    if (tipo == "objeto")
                    {
                        var readTask = responseTask.Content.ReadAsAsync<Usuario>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        var readTask = responseTask.Content.ReadAsAsync<IList<Usuario>>();
                        readTask.Wait();

                        return readTask.Result;
                    }

                }
                else
                {
                    return null;
                }
            }
        }

        public static bool EnviandoMensajes(string controlador, Conversaciones nuevoMensaje)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalHostPort);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = client.PostAsJsonAsync(controlador, nuevoMensaje))
                {
                    if (response.Result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static void ActualizandoDescarga(string controlador, Conversaciones actualizado)
        {
            using (var client = new HttpClient())
            {
                var direccion = $"{LocalHostPort}Conversaciones/{controlador}";
                client.BaseAddress = new Uri(direccion);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var putTask = client.PutAsJsonAsync(controlador, actualizado);
                putTask.Wait();
                var result = putTask.Result;
            }
        }

        public static bool BorrandoMensaje(string controlador)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalHostPort);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = client.DeleteAsync(controlador))
                {
                    if (response.Result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static object ObteniendoMensajes(string controlador)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIConnection.LocalHostPort);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseTask = client.GetAsync(controlador).Result;

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Content.ReadAsAsync<IList<Conversaciones>>();
                    readTask.Wait();

                    return readTask.Result;
                }
                else
                {
                    return null;
                }
            }
        }
    }

}
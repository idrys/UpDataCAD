using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public class ConnectServer
    {
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjNmYzIyNzBlODViOTU5NjhiNjJlZWI1NDJjNTliMTZjNDQ4MjYyMmM4NzllMTRjOGIzN2RjOGRmMWUwZTMwZGI3NmMyMWQwNDVlMTllYTJhIn0.eyJhdWQiOiIzIiwianRpIjoiM2ZjMjI3MGU4NWI5NTk2OGI2MmVlYjU0MmM1OWIxNmM0NDgyNjIyYzg3OWUxNGM4YjM3ZGM4ZGYxZTBlMzBkYjc2YzIxZDA0NWUxOWVhMmEiLCJpYXQiOjE1MTIzOTkzODEsIm5iZiI6MTUxMjM5OTM4MSwiZXhwIjoxNTQzOTM1MzgxLCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.pWTvSqcicdlEpuw2m-mQTLlRctvRh0u9hQ4GBGDKtf9NY0Mz6ad0Y31VIcCwUp_9-Ji8OOLA5r08C9tPo5Q17LTvLG6ctyXkucgjTVDOnam2-WxYazDkXWFKZ9Nq4e13H8KJ6ltRzYlLvzS2fXT9VxeYW5KAObEMYeIRhAS_LxjFc74curjzp82hJQ0k7sG0GPl-ex37gp_r-W9N8ZbjuJ4yixbg0iDlI1SZBaPca51edLj3OZUqlV6L69hBqJsVpJB97cwQCIutcFb_y0l6upfDIlNHwwKOeSlzfTEr0i7q1f6uWYB4_9g7tjJWlXTQnGilTJEThxkGWevWXgO2ZoDwnko4Aa370HwZAIj7VxW_YI3lwj6XlQuNqIfA3Z6gnDd-H8JNRL0Qc3elT28SOPuzU_RUE4JeT49sE2kzbtgDNl3SP9-Z5mNHYBve2gIDyTFLKhtc5R6zgVeGUisEt3UVrfSWywG2i7nJcd8Hhory9GtjbkMnLUnbsElY2hZUyDtxt9CjVC7CuQbrOPv-GIbXg2P9dGLkUEGkODMTYtJvP6MJvEnlh8Qx-AIny-FfTXvMb7PWoZYLybgJ94mYV5izSjG7feo4ZwgdPfFv42LKHqjkRYUI3KJuhjDiZHIJD3gFu2wfP8jg-pQeyoyOKKssPokffSGs-0RkWR5Pdnc";
        string page;
        public ConnectServer(string _address)
        {
            page = _address;
        }

        public async void GetUsers()
        {
            Json json = new Json();
            HttpClient client = GetClient(token);

            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null &&
                    result.Length >= 50)
                {
                    json.Serializer(result);


                }
            }
        }

        /// <summary>
        /// Wysyłanie danych na serwer
        /// </summary>
        /// <param name="information">dane w formacie JSON</param>
        public async void SendInformation(string information )
        {
            Json json = new Json();

            information = "'desktop':'386', 'name': 'Jan', 'lastname': 'Nowak', 'email':'jan.nowak@tgs.pl', 'phone':'626111221' 'department':'Ruda Śląska', 'file':'opoczno.exe', 'start':'21-01-2017 10:00:00', 'end':'21-01-2017 10:08:32'";

            var jsonUser = JsonConvert.SerializeObject(information);

            HttpClient client = GetClient(token);

            StringContent content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(page + "api/user", content);

            var responseString = response.Content.ReadAsStringAsync().Result;
        }


        /// <summary>
        /// Sprawdzenie certyfikatu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            if (error == System.Net.Security.SslPolicyErrors.None)
                return true;
            //MessageBox.Show(cert.Subject, error.ToString());
            Debug.WriteLine("X509Certificate [{0}] Policy Error '{1}'",
                cert.Subject,
                error.ToString());

            return true;
        }

        private static HttpClient GetClient(string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);

            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;


            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };
            return client;
        }
    }
}

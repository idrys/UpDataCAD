using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        string token = File.ReadAllText("ta.txt");
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

            //information = "'desktop':'386', 'name': 'Jan', 'lastname': 'Nowak', 'email':'jan.nowak@tgs.pl', 'phone':'626111221' 'department':'Ruda Śląska', 'file':'opoczno.exe', 'start':'21-01-2017 10:00:00', 'end':'21-01-2017 10:08:32'";

            var jsonUser = JsonConvert.SerializeObject(information);

            HttpClient client = GetClient(token);

            StringContent content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

            try
            {
               // HttpResponseMessage response = await client.PostAsync(page + "api/user", content);
                HttpResponseMessage response =  client.PostAsync(page + "api/user", content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Nie udało się nawiązać praidłowego połaczenia z serverem.");
                }
                var responseString = response.Content.ReadAsStringAsync().Result;
            }
            catch(Exception ex)
            {
                throw new Exception("Nie udało się wysłać danych klienta: " + ex.Message);
            }

            
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

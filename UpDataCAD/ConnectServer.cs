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
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6ImVmYzFjOTc0NmI1ZWIwODljYTA1NDM2ZWNlN2QxZWE0MDNiMDFmNTIyZmJkYzYzY2U4OWU5YjEyNzFmYTdhNWViNDdmNTRmZjBiOTQ1ZGU3In0.eyJhdWQiOiIzIiwianRpIjoiZWZjMWM5NzQ2YjVlYjA4OWNhMDU0MzZlY2U3ZDFlYTQwM2IwMWY1MjJmYmRjNjNjZTg5ZTliMTI3MWZhN2E1ZWI0N2Y1NGZmMGI5NDVkZTciLCJpYXQiOjE1MTc4MjUzODgsIm5iZiI6MTUxNzgyNTM4OCwiZXhwIjoxNTQ5MzYxMzg4LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.lnvquBEGB0g3-82ztA1iT4GthlXOyO5zX3qiW7iDT6D1cM15_Zn6slk1UOcHR1tsC406Px45eXvTZUjl3lW74Ynt11lCm7isygO-aQNweBA_Oyw_3ArcyjSvWmtEaYVVqAhivnNnJySJGMaUSVxNirPCTjagtcL31LIBR18IsUp9828-XvqGPwMk8jf0I2JHTPwVzCpeuFqQpvatkrJ_zJ4qPrvsWqMrX5tkPhuV9MhUKouiNLkbikbrjqC-43Inzs5nhoMhY2dssBj9rhfQUUyDip15-eWHNCFk_1SoSLl_lwa5R3gPMundtFdCMD_q4OdCrj5xzYt2gnsrh554tV-M3aYcqcphko6iQd_sq8d1Q4916iIn9tVa9IfWxSicyNCEglgJCaaZ9zrmeLeaSoqbhFjNpmQ_-3xlZUYY7nKhKET6_3PTulNgb7eeFHIi6Qe3bxJKDFZuSQ6CH3oZJcHNrX1XcQVOoqLthWqSBWRdbxLleVPjEMPnWBj7Y9CXDO_Dr8AcIrkrTl2-hjzrhFMfxjnxOinEMnXooMqucyTFt3BC1REyeUyvuJJudwUdHOqxPDJXVTEQo7G4J_tB5HwCt6moenCi8HG8TtsTnQUY477b_QjosQYZ2HJyG9RPBcmoxySaEj0YO5Hh8-mRI1BIRfrs21MIWjmnCscH5_c";
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

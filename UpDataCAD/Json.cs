using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace UpDataCAD
{
    public class Json
    {
        string filePath;
        private List<UpdatePath> jsonData;
        public List<UpdatePath> Data
        {
            get { return jsonData; }
        }
        public Json(string path)
        {
            filePath = path;
            
        }
        public Json()
        {
            jsonData = new List<UpdatePath>();
        }


        /// <summary>
        /// Zapisywanie listy json do wskazanego pliku
        /// </summary>
        /// <param name="jsonList">Lista json z danymi</param>
        /// <param name="path">Scieżka do pliku w którym dane mają być zapisane</param>
        public void Save(List<UpdatePath> jsonList, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();

                serializer.Serialize(file, jsonList);
            }
        }

        /// <summary>
        /// Zapisywanie listy json do wskazanego pliku podczas deklarowania klasy
        /// </summary>
        /// <param name="jsonList">Scieżka do pliku w którym dane mają być zapisane</param>
        public void Save(List<UpdatePath> jsonList)
        {
            if (filePath == string.Empty)
                throw new Exception("Nie wskazano miejsca zapisu pliku z danymi JSON");

            this.Save(jsonList, filePath);
        }

        /// <summary>
        /// Zapisywanie listy json do wskazanego pliku podczas deklarowania klasy
        /// </summary>
        public void Save()
        {
            if (filePath == string.Empty)
                throw new Exception("Nie wskazano miejsca zapisu pliku z danymi JSON");

            if(this.jsonData.Count == 0)
                 throw new Exception("Brak danych JSON do zapisu!");

            this.Save(this.jsonData, filePath);

        }

        /// <summary>
        /// Odczyt danych z pliku lub adresu Url i konwersja na json
        /// </summary>
        /// <param name="path">Ścieżka do pliku</param>
        /// <returns></returns>
        public List<UpdatePath> Read( string path )
        {
            Uri uriResult;
            bool result = Uri.TryCreate(path, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
                return ReadFromWeb(path);
            else
                return ReadLocal(path);

               
        }

        /// <summary>
        /// Wyszukuje pozycji w liście
        /// </summary>
        /// <param name="id">id szukanego elementu</param>
        /// <returns></returns>
        public  List<UpdatePath> Find(int id)
        {
            return jsonData.FindAll(s => s.ID == id.ToString());
        }

        /// <summary>
        /// Odczyt danych z strumienia Sream i konwersja na json
        /// </summary>
        /// <param name="path">Ścieżka do pliku</param>
        /// <returns></returns>
        public List<UpdatePath> Read(Stream path)
        {
            List<UpdatePath> jsonList;
            
            JsonSerializer serializer = new JsonSerializer();
            StreamReader reader = new StreamReader(path);
            string jsonString = reader.ReadToEnd();
            jsonList = JsonConvert.DeserializeObject<List<UpdatePath>>(jsonString);

            path.Close();

            this.jsonData = jsonList;
            return jsonList;
        }

        /// <summary>
        /// Aktualizacja wybranego elementu JSON
        /// </summary>
        /// <param name="id">ID wybranego elementu</param>
        /// <param name="newData">Nowy dane wybranego elementu</param>
        public void Update(int id, UpdatePath newData)
        {
            UpdatePath item = jsonData.Find(s => s.ID == id.ToString());
            item.LocalPath = newData.LocalPath;
            item.Name = newData.Name;
            item.Update_at = newData.Update_at;
            item.WebPath = newData.WebPath;
            item.ControllFile = newData.ControllFile;
        }

        /// <summary>
        /// Aktualizacja tylko daty
        /// </summary>
        /// <param name="id">ID elementu JSON</param>
        /// <param name="date">Nowa wartość daty</param>
        public void UpdateDate(int id, string date)
        {
            UpdatePath item = jsonData.Find(s => s.ID == id.ToString());
            item.Update_at = date;
        }

        /// <summary>
        /// Usunięcie wybranego elementu
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            UpdatePath item = jsonData.Find(s => s.ID == id.ToString());
            jsonData.Remove(item);
        }

        /// <summary>
        /// Dodanie nowego elementu
        /// </summary>
        /// <param name="newData">Nowy element</param>
        public void Add(UpdatePath newData)
        {
            //newData.ID = jsonData.Last().ID+1; Wywaliłem to bo jeśli najwierw usunę jakiś element a potem dodam nowy to całość się rozjedzie
            jsonData.Add(newData);
        }

        /// <summary>
        /// Porównanie domyślnego JSON ze wskazanym w parametrze
        /// </summary>
        /// <param name="jsonToCheck">JSON do porównania</param>
        /// <returns>Zwraca elementy które się różnią</returns>
        public List<UpdatePath> Compare(List<UpdatePath> jsonToCheck)
        {
            List<UpdatePath> differences = new List<UpdatePath>();

            // Jeśli jsonData jest pusty ( brak danych ) wtedy przygotowujemy wszystkie pliki do aktualizacji
            if (jsonData.First().ID == null)
            {
                foreach (var item in jsonToCheck)
                    differences.Add(item);
                return differences;
            }

            

            // Sprawdzam czy są jakieś różnice między tymi samymi elementami
            foreach (var item in jsonData)
            {
                //Debug.WriteLine(item.ID);
                UpdatePath web = jsonToCheck.Find(s => s.ID == item.ID);
                if (web.Update_at != item.Update_at)
                    differences.Add(web);

            }

            //Sprawdzam czy są nowe elementy
            foreach (var item in jsonToCheck)
            {
                //Debug.WriteLine(item.ID);
                UpdatePath tmp = this.Data.Find(s => s.ID == item.ID);
                if (tmp == null)
                    differences.Add(item);
            }

            return differences;
        }

        /// <summary>
        /// Porównuje i usuwa elementy, których nie ma w Liście jsonToChack
        /// </summary>
        /// <param name="jsonToCheck">Lista elementów, która jest wzorcem</param>
        public void CompareAndRemove(List<UpdatePath> jsonToCheck)
        {
            if (jsonData.First().ID == null)
                return;

            List<UpdatePath> toRemove = new List<UpdatePath>();
           

            foreach (var item in jsonData)
            {
                
                UpdatePath tmp = jsonToCheck.Find(s => s.ID == item.ID);
                if (tmp == null)
                    toRemove.Add(item);
            }

            foreach (var item in toRemove)
            {
                this.Remove(item.LP);
            }

        }

        /// <summary>
        /// Porównuje i usuwa elementy, których nie ma w Liście jsonToChack
        /// </summary>
        /// <param name="jsonToCheck">Lista elementów, która jest wzorcem</param>
        public List<UpdatePath> CompareToAdd(List<UpdatePath> jsonToCheck)
        {
            List<UpdatePath> toAdd = new List<UpdatePath>();

            foreach (var item in jsonToCheck)
            {
                //Debug.WriteLine(item.ID);
                UpdatePath tmp = this.Data.Find(s => s.ID == item.ID);
                if (tmp == null)
                    toAdd.Add(item);
            }
            return toAdd;
            
        }

        /// <summary>
        /// Porównuje i usuwa elementy, których nie ma w Liście jsonToChack
        /// </summary>
        /// <param name="jsonToCheck">Lista elementów, która jest wzorcem</param>
        public List<UpdatePath> CompareToAdd(UpdatePath jsonToCheck)
        {
            List<UpdatePath> toAdd = new List<UpdatePath>();

            
                UpdatePath tmp = this.Data.Find(s => s.ID == jsonToCheck.ID);
                if (tmp == null)
                    toAdd.Add(jsonToCheck);

            return toAdd;

        }

        /// <summary>
        /// Odczytuje plik ze ścieżki lokalnej, sprawdza i wprowadza do listy
        /// </summary>
        /// <param name="path">Ściżka do pliku JSON</param>
        /// <returns>Lista pozycji z JSON</returns>
        private List<UpdatePath> ReadLocal(string path)
        {
            List<UpdatePath> jsonList;
            using (Stream file = (Stream)File.OpenRead(path))
            {
                jsonList = Serializer(file);

            }
            this.jsonData = jsonList;
            return jsonList;
        }

        /// <summary>
        /// Odczytuje plik ze ścieżki web, sprawdza i wprowadza do listy
        /// </summary>
        /// <param name="webPathJson"></param>
        /// <returns></returns>
        private List<UpdatePath> ReadFromWeb(string webPathJson)
        {
            List<UpdatePath> jsonList;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            WebClient client = new WebClient();
            try
            {
                client.OpenRead(webPathJson);
            }catch(Exception ex)
            {
                string f = ex.Message;
            }
            using (Stream dataWeb = client.OpenRead(webPathJson))
            {
                jsonList = Serializer(dataWeb);
            }

            this.jsonData = jsonList;

            return jsonList;
        }

        /// <summary>
        /// Certificate validation callback.
        /// </summary>
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                error.ToString());

            return false;
        }

        /// <summary>
        /// Sprawdza podany ciąg pod względem poprawności formatu JSON
        /// </summary>
        /// <param name="s">Zwraca warość ciągu JSON w liście</param>
        /// <returns></returns>
        public List<UpdatePath> Serializer(Stream s)
        {
            JsonSerializer serializer = new JsonSerializer();
            StreamReader reader = new StreamReader(s);
            string jsonString = reader.ReadToEnd();
            UpdatePath up = new UpdatePath();
            up.Path = "/code/test.zip";

            return JsonConvert.DeserializeObject<List<UpdatePath>>(jsonString);
            
        }

        public void Serializer(string jsonString)
        {
            JsonSerializer serializer = new JsonSerializer();            
            var jsonData = JsonConvert.DeserializeObject<Who>(jsonString);
            
        }
    }
}

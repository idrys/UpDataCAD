using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        /// Odczyt danych z pliku i konwersja na json
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
        /// Odczyt danych z pliku i konwersja na json
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
            item.Date = newData.Date;
            item.WebPath = newData.WebPath;
        }

        /// <summary>
        /// Aktualizacja tylko daty
        /// </summary>
        /// <param name="id">ID elementu JSON</param>
        /// <param name="date">Nowa wartość daty</param>
        public void UpdateDate(int id, string date)
        {
            UpdatePath item = jsonData.Find(s => s.ID == id.ToString());
            item.Date = date;
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
            newData.ID = jsonData.Last().ID+1;
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

            foreach (var item in jsonData)
            {
                //Debug.WriteLine(item.ID);
                UpdatePath web = jsonToCheck.Find(s => s.ID == item.ID);
                if (web.Date != item.Date)
                    differences.Add(web);

            }

            return differences;
        }

        private List<UpdatePath> ReadLocal(string path)
        {
            List<UpdatePath> jsonList;
            using (Stream file = (Stream)File.OpenRead(path))
            {
                jsonList = Serializer(file);

            }
            return jsonList;
        }

        private List<UpdatePath> ReadFromWeb(string webPathJson)
        {
            List<UpdatePath> jsonList;

            WebClient client = new WebClient();

            using (Stream dataWeb = client.OpenRead(webPathJson))
            {
                jsonList = Serializer(dataWeb);
            }
            return jsonList;
        }

        private List<UpdatePath> Serializer(Stream s)
        {
            JsonSerializer serializer = new JsonSerializer();
            StreamReader reader = new StreamReader(s);
            string jsonString = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<UpdatePath>>(jsonString);
            // Wyjątek pojawia się prawdopodobnie bo na stronie plik json ma ciągle wpisane "Table"
        }
    }
}

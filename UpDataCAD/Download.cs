using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{
    public class Download
    {
        //string t = Properties.Settings.Default.PathToRepo;
        //Configuration config = new Configuration();
        //FileParts[] webFilesToDownload;
        Json jsonWeb;
        Json jsonLocal;

       // List<UpdatePath> jsonWeb;
       // List<UpdatePath> jsonLocal;

        private string webPathJson = "https://1sw.pl/caddecor/update/path.json";
        private string localPathJson;

        public Download(string _localPathJson)
        {
            jsonWeb = new Json();
            
            localPathJson = _localPathJson + "path.json";
            if (!File.Exists(localPathJson))
            {
                File.Create(localPathJson);
            }

            jsonLocal = new Json(localPathJson);
        }
        
        /*
        public FileParts[] WebFilesToDownload
        {
            get { return webFilesToDownload; }
        }
        

        /// <summary>
        /// Popbierz aktualną ścieżkę do bazy
        /// </summary>
        public List<FileParts> GetCurrentPath()
        {
            List<FileParts> currentPath = new List<FileParts>();

            FileParts[] tmpFiles = null;

            WebClient client = new WebClient();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            foreach (var item in config.Update)
            {
                tmpFiles = ReadPath(doc, client, item.Page, item.Path);
                //Debug.WriteLine(tmpFiles.First().ToString());
                foreach (var itemFile in tmpFiles)
                {
                    //Debug.WriteLine(itemFile);
                    currentPath.Add(itemFile);
                }
            }
            return currentPath;
        }
        */
        /// <summary>
        /// Odczytuje pełną ścieżkę i nazwę pliku z aktualizacją bazy danych ze wskazanej strony web
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="client"></param>
        /// <param name="updatePage">To adres strony na której może się znajdować kilka baz z aktualizacjami</param>
        /// <param name="updatePath">To CZĘŚĆ ścieżki do bazy ( bez nazwy pliku, bo ten zmienia się przy każdej aktualizacji )</param>
        /// <returns>Zwraca listę plików dostępnej na stronie do ściągnięcia</returns>
        private FileParts[] ReadPath(HtmlAgilityPack.HtmlDocument doc, WebClient client, string updatePage, string updatePath)
        {
            
            List<FileParts> files = new List<FileParts>();
            doc.Load(client.OpenRead(updatePage));
            
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                
                if (link.Attributes["href"].Value.IndexOf(updatePath) == 0)
                {
                    FileParts tmpfiles = new FileParts();
                    tmpfiles.GetExtantion(link.Attributes["href"].Value);
                    files.Add(tmpfiles);
                }

            }
            return files.ToArray<FileParts>();
        }

        /// <summary>
        /// Porównaj Lokalną bazę z bazą na stronie web
        /// </summary>
        /// <returns>Zwraca TRUE jeśli jest nowa baza</returns>
        public FileParts[]  Compare(string[] localFiles, FileParts[] webFiles)
        {
            
            if (localFiles.Length == 0)
                return webFiles.Where(s => s.Extention == "exe").ToArray(); 
           

            //Porównanie lokalnych plików z listą plików umieszczonych na stronie internetowej
            foreach (var localfile in localFiles)
            {
                //Debug.WriteLine(localfile);
                webFiles = webFiles.Where(s => s.FullName == localfile).ToArray();
            }

            // Odfiltrowanie tylko plików z rozrzeżeniem "*.exe"
            webFiles = webFiles.Where(s => s.Extention == "exe").ToArray();

            return webFiles;
        }

        private List<UpdatePath> CreateStructure(Stream data)
        {
            StreamReader reader = new StreamReader(data);
            string json = reader.ReadToEnd();

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
           
            DataTable dataTable = dataSet.Tables["Table"];

            List<UpdatePath> path = new List<UpdatePath>();
            foreach (DataRow row in dataTable.Rows)
            {
                UpdatePath p = new UpdatePath();
                p.ID = row["ID"].ToString();
                p.Name = row["Name"].ToString();
                p.Date = row["Date"].ToString();
                p.WebPath = row["WebPath"].ToString();
                p.LocalPath = row["LocalPath"].ToString();
                path.Add(p);
            }
            return path;
        }

        /// <summary>
        /// Informuje o nowej aktualizacji
        /// </summary>
        /// <returns>Zwraca TRUE jeśli jest nowa baza</returns>
        public List<UpdatePath> IsNewUpdate()
        {
            List<UpdatePath> filesToUpdate = new List<UpdatePath>();

            jsonWeb.Read(webPathJson);
            jsonLocal.Read(localPathJson);

            jsonLocal.CompareAndRemove(jsonWeb.Data);
            //jsonLocal.CompareAndAdd(jsonWeb.Data);
            jsonLocal.Save();

            filesToUpdate = jsonLocal.Compare(jsonWeb.Data);

            return filesToUpdate;
           
        }
        
        public void UpdatedJson(string ID, string newDate)
        {
            jsonLocal.UpdateDate(int.Parse(ID), newDate);
            jsonLocal.Save();           
        }

        public void UpdatedJson(UpdatePath newData)
        {
            // Sprawdzam czy newData to zupełnie nowy element czy tylko aktualizacja istniejących danych
            if (jsonLocal.CompareToAdd(newData).Count == 0)
                jsonLocal.UpdateDate(newData.LP, newData.Date);
            else
                jsonLocal.Add(newData);

            jsonLocal.Save();
        }


        public void CreateJosnItem(UpdatePath item)
        {
            ///TODO


            /*
            StreamReader reader = new StreamReader( Stream.Null );
            string json = reader.ReadToEnd();

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
            DataTable dataTable = dataSet.Tables["Table"];

            List<UpdatePath> path = new List<UpdatePath>();
            foreach (DataRow row in dataTable.Rows)
            {
                UpdatePath p = new UpdatePath();
                p.ID = row["ID"].ToString();
                p.Name = row["Name"].ToString();
                p.Date = row["Date"].ToString();
                p.WebPath = row["WebPath"].ToString();
                p.LocalPath = row["LocalPath"].ToString();
                path.Add(p);
            }
            */

            throw new NotImplementedException("Nie zrobiłem implementacji CreateJosnItem");
        }

       


/*
        public void DownloadFiles()
        {
            //Debug.WriteLine(webFilesToDownload.First().FullName);
            //DownloadFiles(webFilesToDownload);
            DownloadFilesThread(webFilesToDownload);
        }
        
        private void DownloadFilesThread(FileParts[] filesToDownload)
        {
            WebClient client = new WebClient();
            string url = filesToDownload.Last().FullPath;
            if (!string.IsNullOrEmpty(url))
            {
                Thread thread = new Thread(() =>
                {
                    Uri uri = new Uri(url);
                    string fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                    DowloadLargeFile(filesToDownload.First().FullPath, filesToDownload.First().FullName);
                });
                thread.Start();
            }
        }
        /*
        private void DownloadFiles(FileParts[] filesToDownload)
        {
            WebClient myWebClient = new WebClient();
            int i = 0;
            foreach (var item in filesToDownload)
            {
                
                //Debug.WriteLine("Ściągam plik: " + item.FullPath + "; " + item.FullName);
                
                    DowloadLargeFile(item.FullPath, item.FullName);
                   
            }
            
        }
        /*
        private void DowloadLargeFile(string fullpath, string fileName)
        {
            DateTime startTime = DateTime.UtcNow;
            WebRequest request = WebRequest.Create(fullpath);
            WebResponse response = request.GetResponse();
           
            using (Stream responseStream = response.GetResponseStream())
            {

               
                using (Stream fileStream = File.OpenWrite(@"c:\Users\Admin\tmp\" + fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = responseStream.Read(buffer, 0, 4096);
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        DateTime nowTime = DateTime.UtcNow;
                        if ((nowTime - startTime).TotalMinutes > 5)
                        {
                            throw new ApplicationException(
                                "Download timed out");
                        }
                        bytesRead = responseStream.Read(buffer, 0, 4096);
                    }
                }
            }
            
        }
        */
    }
}

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{
    public class Download
    {
        //string t = Properties.Settings.Default.PathToRepo;
        Configuration config = new Configuration();
        FileParts[] webFilesToDownload;

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
                foreach (var itemFile in tmpFiles)
                {
                    currentPath.Add(itemFile);
                }
            }
            return currentPath;
        }

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
            if (localFiles == null)
                return webFiles;

            //Porównanie lokalnych plików z listą plików umieszczonych na stronie internetowej
            foreach (var localfile in localFiles)
            {               
                webFiles = webFiles.Where(s => s.FullName == localfile).ToArray();
            }

            // Odfiltrowanie tylko plików z rozrzeżeniem "*.exe"
            webFiles = webFiles.Where(s => s.Extention == "exe").ToArray();

            return webFiles;
        }

        /// <summary>
        /// Informuje o nowej aktualizacji
        /// </summary>
        /// <returns>Zwraca TRUE jeśli jest nowa baza</returns>
        public bool IsNewUpdate()
        {
            FileParts[] webFiles = GetCurrentPath().ToArray();
            
            webFilesToDownload = Compare(config.LocalFiles, webFiles);
            
            if (webFilesToDownload.Count() == 0)
                return false;
            else
                return true;               
        }

        public void DownloadFiles()
        {
            DownloadFiles(webFilesToDownload);
        }

        private void DownloadFiles(FileParts[] filesToDownload)
        {
            WebClient myWebClient = new WebClient();
            foreach (var item in filesToDownload)
            {
                myWebClient.DownloadFile(item.FullPath, item.FullName);
            }
            
        }
    }
}

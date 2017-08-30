using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace UpDataCAD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string baseURL = "http://opoczno.eu/uploads/";
            WebClient client = new WebClient();
            string content = client.DownloadString(baseURL);
            WebBrowser w = new WebBrowser();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            

            //doc = html;
           // doc.Load(baseURL);
            doc.Load(client.OpenRead("http://www.opoczno.eu/o-opoczno/#!/dla-architektow-i-inwestorow/"));
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]") )
            {
                //Console.WriteLine(link.Attributes["href"].Value);
                if(link.Attributes["href"].Value.IndexOf("http://opoczno.eu/uploads/") == 0)
                MessageBox.Show(link.Attributes["href"].Value);
            }

            //MessageBox.Show(content);

        }




        /// <summary>
        /// Odczytuje listę wszystkich plików znajdujących się w repozytorium
        /// </summary>
        /// <param name="folderName">Ścieżka do folderu w którym znajdują się pliki do rozpakowania</param>
        /// <returns>Zwraca LISTE pełnych ścierzek do wszystkich plików w folderze</returns>
        static string[] ReadListFilesFromRepository(string folderName)
        {

            string[] fileEntries = null;
            if (Directory.Exists(folderName))
            {
                // Process the list of files found in the directory.
                fileEntries = Directory.GetFiles(folderName);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(folderName);
            }
            else
            {
                Console.WriteLine("{0} to niej prawidłowa sciezka do katalogu.", folderName);
            }

            return fileEntries;
        }

        private void OnCreate(object sender, EventArgs e)
        {
            string[] list = ReadListFilesFromRepository("c:\\CADProject\\Repo");
            MessageBox.Show("");
        }
    }
}

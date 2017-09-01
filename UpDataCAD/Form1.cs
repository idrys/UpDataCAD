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
using Newtonsoft.Json;

namespace UpDataCAD
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Klasa z nazwami w osobnych strumieniach, nazwa rozrzeszenie, ścieżka i pełna nazwa pliku
        /// </summary>
        public class FileParts
        {
            public string FullPath = string.Empty;
            public string FullName = string.Empty;
            public string OnlyName = string.Empty;
            public string Extention = string.Empty;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Download d = new Download();
            MessageBox.Show( "Czy są jakieś pliki do aktualizacji: " + d.IsNewUpdate().ToString());
            //d.DownloadFiles();
            
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

        /// <summary>
        /// Metoda wykonywana przed utworzeniem okna dialogowego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreate(object sender, EventArgs e)
        {
            string[] list = ReadListFilesFromRepository("c:\\CADProject\\Repo");
            //MessageBox.Show("");
        }


    }
}

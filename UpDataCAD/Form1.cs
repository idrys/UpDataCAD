using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{


    public partial class Form1 : Form
    {

        private Download d;
        string tmpFolder;
        string pathToCadProject;
        string tgsCadProjektFolder;

        List<UpdatePath> list_updatePathh;
        private bool downloadComplete = false;
        string nameFile = "";

        public Form1()
        {
            // Główny folder programu w, którym będą trzymane pliki konfiguracyjne
            tgsCadProjektFolder = GetPathToFolder(@"\TGS\CADDecor\");

            //Ustawienie ścieżki TMP, do którego będą ściągane pliki z web
            tmpFolder = GetPathToFolder(@"\TGS\CADDecor\tmp\");

            // Wszystko co jest związane ze ściąganiem plików
            d = new Download(tgsCadProjektFolder);

            // Ścieżka do CadProject z plikiem iUPDATE.exe
            pathToCadProject = GetPathToCadProjekt();
                      
            // Sprawdzam czy jest uruchomoiny programicad.exe
            CheckCadProjektRun();

            // Wczytuję listę dostępnych aktualizacji
            list_updatePathh = d.IsNewUpdate();

           
            InitializeComponent();
            label1.Text = "Sprawdzam ...";
            if (list_updatePathh.Count == 0)
            {
                btnUpdate.Text = "Zamknij";
                label2.Text = "Nie ma nowych danych do aktualizacji";
            }

        }

        private string GetPathToFolder(string path)
        {
            string tmp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + path;

            if (!Directory.Exists(tmp))
                Directory.CreateDirectory(tmp);

            return tmp;
        }
       
        /// <summary>
        /// Określa ścieżkę do CadProjekt oczytując dane z Regedit
        /// Jeśli się nie uda uruchamia okno dialogowe z folderami
        /// </summary>
        private string GetPathToCadProjekt()
        {
            string path = GetPathCADProjektFromRegistry(Registry.CurrentUser);

            if (!File.Exists(path + "\\iUPDATE.exe") || (path.Length == 0))
            {

                path = GetPathCADProjektFromRegistry(Registry.LocalMachine);


                if (path.Length == 0)
                    path = FolderBrowserDialog();

                while (!File.Exists(path + "\\iUPDATE.exe"))
                {
                    DialogResult result = DialogResult.Retry;
                    result = MessageBox.Show("Nie wskazano prawidłowej ścieżki do CADDecor. Musi się tam znajdować plik iUPDATE.exe", "Wskaż CADDecor", MessageBoxButtons.RetryCancel);
                    if (result == DialogResult.Cancel)
                    {
                        Debug.WriteLine("Abort");
                        System.Windows.Forms.Application.Exit();
                        Environment.Exit(0);
                    }

                    if (result == DialogResult.Retry)
                    {
                        Debug.WriteLine("Retry");
                        path = FolderBrowserDialog();
                    }

                }

                RegistryKey key = Registry.CurrentUser;
                RegistryKey software = key.CreateSubKey(@"SOFTWARE\\CADPROJEKT\INSTALLATIONS\0\");
                software.SetValue("PATH", path);
            }
            return path;
        }

        /// <summary>
        /// Uruchomienie aktualizacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (list_updatePathh.Count != 0)
                FireDownloadQueue(list_updatePathh, tmpFolder);
            else
            {
                System.Windows.Forms.Application.Exit();
            }

            label2.Text = "Aktualizacja zakończona";
            
        }      
     
        /// <summary>
        /// Sprawdza czy w tle działa CADDECOR
        /// </summary>
        public void CheckCadProjektRun()
        {
            string message = "Aby uruchomić aktualizacje musisz zamknąć CADDecor-a";
            string heading = "Zamknij CADDEcor-a";

            Process[] processeseodsa = Process.GetProcesses();
            

            DialogResult result = DialogResult.Retry;
            while (processeseodsa.Any(k => k.ProcessName == "icad.exe"))
            {

                result = MessageBox.Show(message, heading, MessageBoxButtons.RetryCancel);
                if (result == DialogResult.Cancel)
                {
                    Debug.WriteLine("Abort");
                    System.Windows.Forms.Application.Exit();
                    Environment.Exit(0);
                }

                if (result == DialogResult.Retry)
                {
                    Debug.WriteLine("Retry");
                }

            }
        }

        private void ExtractFile(string pathToFile, string pathToDestFolde)
        {
            // Download d = new Download();
            ProgressAsyncInvoke t = new ProgressAsyncInvoke(pathToFile, pathToDestFolde);
            try
            {

                AsyncInvoke method1 = t.ExtractFile;
                IAsyncResult asyncResult = method1.BeginInvoke(null, null);

                while (!asyncResult.IsCompleted)
                {
                    //if (t.Progress() == 0)
                    //Thread.Sleep(100);
                    //Application.DoEvents();
                    if (t.Progress() == 0)
                    {
                        label2.Text = "Konfiguruję plik, czekaj ...";
                        label2.Refresh();
                        //Debug.WriteLine(t.Progress());
                    }
                    else
                    {
                        label2.Text = "";
                        label2.Refresh();
                    }
                    //Debug.WriteLine(t.Progress());

                    progressBar1.Value = t.Progress();
                }
                progressBar1.Value = 100; // potrzebne bo czasem 7z pokazuje tylko 95%
                
                t.Zero();

                int retVal = method1.EndInvoke(asyncResult);
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
                //Debug.WriteLine(ea.ToString());
            }
        }    

        private async void FireDownloadQueue(List<UpdatePath> urls, string tmpF)
        {
            foreach (var url in urls)
            {
                await Task.Run(() => startDownload(url.WebPath, tmpF));
                await Task.Run(() => SevenZipExtractProgress(tmpF + "\\" + url.FileName, pathToCadProject + "\\" + url.LocalPath + "\\", onProgres));
                await Task.Run(() => d.UpdatedJson(url));
                
            }

            btnUpdate.Text = "Zamknij";
            list_updatePathh = new List<UpdatePath>();
        }

        private async void FireExtractQueue(List<UpdatePath> pathToFiles)
        {
            //string repo = "";
            //string cad = "";
            //Debug.WriteLine("Rozpakowywanie.");
            foreach (var pathToFile in pathToFiles)
            {
                await Task.Run(() => SevenZipExtractProgress(tmpFolder + "\\" + pathToFile.FileName, pathToCadProject + "\\" + pathToFile.LocalPath, onProgres));
            }
        }

        private void startDownload(string url, string repo)
        {
            Uri u = new Uri(url);

            //Debug.WriteLine("Sciezka do repo: " + @repo + System.IO.Path.GetFileName(u.LocalPath));
            //Thread thread = new Thread(() =>
            //{

            nameFile = System.IO.Path.GetFileName(u.LocalPath);
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(url), @repo + "\\" + System.IO.Path.GetFileName(u.LocalPath));

            // });
            // thread.Start();

            while (!downloadComplete)
            {
                Application.DoEvents();
            }

            downloadComplete = false;
        }

        private void startDownloadAllNow(string url)
        {

            //Thread thread = new Thread(() =>
            //{
            Uri u = new Uri(url);
            nameFile = System.IO.Path.GetFileName(u.LocalPath);
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(url), @"d:" + System.IO.Path.GetFileName(u.LocalPath));

            // });
            // thread.Start();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            string mask = "### ### ### MB";
            this.BeginInvoke((MethodInvoker)delegate
            {

                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                label2.Text = "Downloaded " + (e.BytesReceived / 1000).ToString(mask) + " of " + (e.TotalBytesToReceive / 1000).ToString(mask);
                label1.Text = nameFile;
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            });

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            this.BeginInvoke((MethodInvoker)delegate
            {
                label2.Text = "Completed";
                progressBar1.Value = 0;
                downloadComplete = true;
            });

        }

        private void onProgres(int uploaded)
        {
            label2.Invoke((MethodInvoker)delegate { label2.Text = "Rozpakowano: " + uploaded.ToString() + "%"; });
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)uploaded; });
        }

        public bool SevenZipExtractProgress(string pathFile, string folderExtract, Action<int> onProgress)
        {
            Regex REX_SevenZipStatus = new Regex(@"(?<p>[0-9]+)%");

            int EverythingOK = -1;
            string testInfo = string.Empty;
            string path7zip = "x86\\7z.exe";

            if (Environment.Is64BitOperatingSystem)
                path7zip = "x64\\7z.exe";

            onProgres(0);

            Process p = new Process();
            p.StartInfo.FileName = path7zip;
            p.StartInfo.Arguments = "e " + "\"" + pathFile + "\"" + " -o\"" + folderExtract + "\"" + " -y -bsp1 -bse1 -bso1";
            p.StartInfo.UseShellExecute = false;    // Nie zbędne do odczytu wartości z wyjścia 7z
            p.StartInfo.RedirectStandardOutput = true;  // Nie zbędne do odczytu wartości z wyjścia 7z

            p.OutputDataReceived += (sender, e) =>
            {    // Odczyt procentów z wyjścia 7z
                if (onProgress != null)
                {
                    Match m = REX_SevenZipStatus.Match(e.Data ?? "");
                    if (m != null && m.Success)
                    {
                        int procent = int.Parse(m.Groups["p"].Value);
                        onProgress(procent); // delegat link do metody                        
                    }
                }
            };

            p.StartInfo.CreateNoWindow = true; // Ukrycie okna

            p.Start();
            p.BeginOutputReadLine(); // Nie zbędne do odczytu wartości z wyjścia 7z
            p.WaitForExit();
            onProgres(100);
            p.Close();

            EverythingOK = testInfo.IndexOf("Everything is Ok");
            return EverythingOK == -1 ? false : true;
        }

        private string GetPathCADProjektFromRegistry(RegistryKey key)
        {

            //RegistryKey key = Registry.LocalMachine;
            
            RegistryKey software = key.OpenSubKey(@"SOFTWARE\\CADPROJEKT\INSTALLATIONS\0\", false);

            if (software != null)
                return software.GetValue("PATH").ToString();
              
            return string.Empty;

        }

        private string FolderBrowserDialog()
        {
            string path = string.Empty;

                var t = new Thread((ThreadStart)(() =>
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
                    fbd.ShowNewFolderButton = false;
                    if (fbd.ShowDialog() == DialogResult.Cancel)
                        return;

                    path = fbd.SelectedPath;
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();

            return path;
        }
    }
}



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

        Who who;
        ConnectServer cs;

        public Form1(Who _who)
        {
            who = _who;
            cs = new ConnectServer("https://1sw.pl/");

            //MessageBox.Show(who.department);
            string tgsCADPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TGS\CADDecor\";

            // Główny folder programu w, którym będą trzymane pliki konfiguracyjne
            tgsCadProjektFolder = GetOrCreatePathToFolder(tgsCADPath);

            //Ustawienie ścieżki TMP, do którego będą ściągane pliki z web
            tmpFolder = GetOrCreatePathToFolder(tgsCADPath + @"\tmp\");
           
            // Wszystko co jest związane ze ściąganiem plików
            d = new Download(tgsCadProjektFolder);
            
            //Porównanie dostępnych plików z aktualizacjami z już zainstalowanymi aktualizacjami 
            try
            {
                d.GetWebJson(); // pobiera plik z listą aktualizacji
                
            }
            catch(Exception ex)
            {
                MessageBox.Show( ex.Message + ". Problem przy pobieraniu listy z dostępnymi aktualizacjami: path.json");
            }
            d.GetLocalJson(); // ładuje lokalną listę aktualizacji

            try
            {
                // Ścieżka do CadProject ( sciezke okreslam szukajac pliku iUPDATE.exe )
                pathToCadProject = GetPathToCadProjekt();
               
                //TODO: Sprawdzamy zainstalowane aktualizacje
                //TODO: CheckLocalFiles(d.JsonWeb, pathToCadProject);
                
                // Sprawdzam czy jest uruchomoiny programicad.exe
                CheckCadProjektRun();

                // Wczytuję listę dostępnych aktualizacji
                list_updatePathh = d.IsNewUpdate();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            InitializeComponent();
            label1.Text = "Lista plików do aktualizacji:";

            foreach (var item in list_updatePathh)
                listBox1.Items.Add(item.FullName);
            

            if (list_updatePathh.Count == 0)
            {
                btnUpdate.Text = "Zamknij";
                label2.Text = "Nie ma nowych danych do aktualizacji";
            }

        }

        

        /// <summary>
        /// Sprawdza daty plików już zainstalowanych w CadDecor i pobiera ich daty ostatnij modyfikacji
        /// Następnie prowadza do jsonLocal
        /// </summary>
        /// <param name="jsonWeb">Dane ze strony Web</param>
        /// <param name="cadPath">Scieżka do instalacji CADDecor</param>
        private void CheckLocalFiles(Json jsonWeb, string cadPath)
        {
            if (jsonWeb == null)
                throw new Exception("Funkcja CheckLocalFiles nie ma wartości jsonWeb! ");
            if (cadPath == string.Empty )
                throw new Exception("Funkcja CheckLocalFiles nie ma wartości cadPath! ");
            if (d.JsonLocal == null)
                throw new Exception("Brak d.JsonLocal");

            foreach (var item in jsonWeb.Data)
            {
                string path = cadPath + "\\" + item.LocalPath + "\\" + item.ControllFile;
                if (File.Exists(path))
                {

                    //TODO: usunąć godzinę z tego wpisu
                    var date = File.GetLastWriteTime(path);

                    if (d.JsonLocal.Find(item.LP).Count == 1)
                        d.JsonLocal.UpdateDate(item.LP, date.ToString());
                    else
                        d.JsonLocal.Add(item);

                }
                d.JsonLocal.Save();
            }
            
        }

        /// <summary>
        /// Sprawdza czy istnieje wskazany foldej, jeśli nie to go tworzy
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetOrCreatePathToFolder(string path)
        {
            //string tmp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + path;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
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

        private string DateTimeMySqlFormat(DateTime dt)
        {
            
            string str = dt.ToString("yyyy-MM-dd H:mm:ss");

            return str;
        }

        /// <summary>
        /// Uruchomienie aktualizacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //who.start = DateTimeMySqlFormat( DateTime.Now );
            //who.file = "filnametest.zip";
            //who.end = DateTimeMySqlFormat (DateTime.Now );
            //cs.SendInformation(who.ToString());

            //  SendInfo(who);
            try
            {
                if (list_updatePathh.Count != 0)
                    FireDownloadQueue(list_updatePathh, tmpFolder);
                else
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        /// <summary>
        /// Rozpakowywanie plików
        /// </summary>
        /// <param name="pathToFile">ścieżka do pliku</param>
        /// <param name="pathToDestFolde">gdzie go rozpakować</param>
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

        /// <summary>
        /// Wątki, które nalerzy pokoleii uruchamiać
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="tmpF"></param>
        private async void FireDownloadQueue(List<UpdatePath> urls, string tmpF)
        {
            foreach (var url in urls)
            {
                await Task.Run(() => who.start = DateTimeMySqlFormat( DateTime.Now ) );
                // Ściąganie pliku
                await Task.Run(() => startDownload(url.Path, tmpF));

                // Rozpakowanie pliku
                await Task.Run(() => SevenZipExtractProgress(tmpF + "\\" + url.FileName, pathToCadProject + "\\" + url.LocalPath + "\\", onProgres));

                // Aktualizacja lokalnego pliku json z informacjami o aktualizacjach
                await Task.Run(() => d.UpdatedJson(url));

                //TODO: Wysłanie na serwer info o udanej aktualizacji
                await Task.Run(() => who.file = url.FileName);
                await Task.Run(() => who.end = DateTimeMySqlFormat( DateTime.Now ));
                await Task.Run(() => SendInfo(who));
            }

            btnUpdate.Text = "Zamknij";
            list_updatePathh = new List<UpdatePath>();
        }

        private void SendInfo(Who who)
        {

            cs.SendInformation(who.ToString());
            //throw new NotImplementedException();
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

            if (uploaded == 100 && listBox1.Items.Count > 0)
                listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Remove(listBox1.Items[0]); });
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

        private void button1_Click(object sender, EventArgs e)
        {
           // who.start = DateTime.Now;
           // who.file = "filnametest.zip";
            //who.end = DateTime.Now;
            //SendInfo(who);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}



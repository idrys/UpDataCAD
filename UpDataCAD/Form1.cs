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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{
    

    public partial class Form1 : Form
    {
       
        private Download d = new Download();
        string repoPath = ConfigurationManager.AppSettings["repo"].ToString();
        string pathDestFolder = ConfigurationManager.AppSettings["cad"].ToString();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FireDownloadQueue(d.IsNewUpdate());

            Extract extract = new Extract();
            string [] listFiles = extract.ReadListFilesFromRepository(repoPath);
            int i = 0;
            foreach (var file in listFiles)
            {
                //Debug.WriteLine("Nazwa pliku " + file);
                //Debug.WriteLine();
                label1.Text = "Plik " + (++i).ToString() + " z " + listFiles.Count();
                label1.Refresh();
                ExtractFile(file, pathDestFolder);
            }
   
        }

        private void ExtractFile(string pathToFile, string pathToDestFolde)
        {
           // Download d = new Download();
            ProgressAsyncInvoke t = new ProgressAsyncInvoke(pathToFile, pathToDestFolde);
            try { 

                AsyncInvoke method1 = t.ExtractFile;
                IAsyncResult asyncResult = method1.BeginInvoke(null, null);

                while (!asyncResult.IsCompleted)
                {
                    //if (t.Progress() == 0)
                    //Thread.Sleep(100);
                    Application.DoEvents();
                    if (t.Progress() == 0)
                    {
                        label2.Text = "Konfiguruję plik, czekaj ...";
                        label2.Refresh();
                        Debug.WriteLine(t.Progress());
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
                Debug.WriteLine(ea.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            
            
            FireDownloadQueue( d.IsNewUpdate() );
        }
        
        

        private bool downloadComplete = false;

        private async void FireDownloadQueue(List<UpdatePath> urls)
        {
            foreach (var url in urls)
            {
                await Task.Run(() => startDownload(url.WebPath));
            }
        }

        string nameFile = "";

        private void startDownload(string url)
        {

            //Thread thread = new Thread(() =>
            //{
            Uri u = new Uri(url);
            nameFile = System.IO.Path.GetFileName(u.LocalPath);
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(url), @repoPath + System.IO.Path.GetFileName(u.LocalPath));

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

            this.BeginInvoke((MethodInvoker)delegate
            {

                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                label2.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                label1.Text = nameFile;
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            });

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            this.BeginInvoke((MethodInvoker)delegate {
                label2.Text = "Completed";
                progressBar1.Value = 0;
                downloadComplete = true;
            });

        }

    }
}


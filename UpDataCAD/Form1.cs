using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            
            string repoPath = ConfigurationManager.AppSettings["repo"].ToString();
            string pathDestFolder = ConfigurationManager.AppSettings["cad"].ToString();

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
            Download d = new Download();
            ProgressAsyncInvoke t = new ProgressAsyncInvoke(pathToFile, pathToDestFolde);
            try { 

                AsyncInvoke method1 = t.ExtractFile;
                IAsyncResult asyncResult = method1.BeginInvoke(null, null);

                while (!asyncResult.IsCompleted)
                {
                    //if (t.Progress() == 0)
                    Thread.Sleep(100);
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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        static int progress = 0;
        
        Thread th;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Download d = new Download();
            //Extract ext = new Extract();
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;

            string pathToFile = @"C:\Users\Slawek\Repo\CAD_Decor_Opoczno_2D_2017-05.exe";
            string pathToFolder = @"C:\CADProject";

            TestAsyncInvoke t = new TestAsyncInvoke();

            AsyncInvoke method1 = t.Method1;
            Debug.WriteLine("Wywołanie metody BeginInvoke w wątku {0}", Thread.CurrentThread.ManagedThreadId);

            IAsyncResult asyncResult = method1.BeginInvoke(null, null);

            Debug.WriteLine("Rozpoczęcie odpytywania w wątku {0}", Thread.CurrentThread.ManagedThreadId);

            while (!asyncResult.IsCompleted)
            {
                Thread.Sleep(10);
                Debug.WriteLine("." + t.Progress().ToString());
                progressBar1.Value = t.Progress();
            }
            progressBar1.Value = 100;
            Debug.WriteLine("Zakończono odpytywanie wątku {0}", Thread.CurrentThread.ManagedThreadId);

            try
            {
                int retVal = method1.EndInvoke(asyncResult);
                Debug.WriteLine("Zwrócono wartość: " + retVal);
            } catch(Exception ea)
            {
                Debug.WriteLine(ea.ToString());
            }

            //ext.SevenZipExtractProgress(pathToFile, pathToFolder , onProgress);
            //Debug.WriteLine("Oczekiwanie na zakończenie wątku...");
        }

       

        private void onProgress(int obj)
        {
            progress = obj;
        }

        public delegate int AsyncInvoke();

        public class TestAsyncInvoke
        {
            static int progress = 0;
            public  int Method1()
            {
                Extract ext = new Extract();

                string pathToFile = @"C:\Users\Slawek\Repo\CAD_Decor_Opoczno_2D_2017-05.exe";
                string pathToFolder = @"C:\CADProject";
                Debug.WriteLine("Wywołano metodę Method1 w wątku {0} ", Thread.CurrentThread.ManagedThreadId) ;
                ext.SevenZipExtractProgress(pathToFile, pathToFolder, onProgress);
                return 5;
            }

            public int Progress()
            {
                return progress;
            }

            private void onProgress(int obj)
            {
                progress = obj;
            }
        }
    }
}

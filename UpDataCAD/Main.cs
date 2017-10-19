using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace UpDataCAD
{
    public class SysTrayApp : Form
    {
        
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

       

        private void ExtractFiles(object sender, EventArgs e)
        {
            Extract ext = new Extract();
            try
            {
                ext.Unpack(@"C:\Users\Slawek\Repo\plytki_cersanit_06_2017.exe");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }
        /*
        private void IsUpdate(object sender, EventArgs e)
        {
            Download d = new Download();
            if (d.IsNewUpdate())
            {
               // MessageBox.Show("Są już dostępne nowe aktualizacje baz danych");
                //d.DownloadFiles();
            }
            else
                MessageBox.Show("Brak nowych aktualizacji");
        }
        */
        private void OnRepoUpdate(object sender, EventArgs e)
        {
            try {
                //Process p = new Process();
                //p.StartInfo.FileName = "AutoUpdataCaD.exe";

                //p.Start();
                Form1 form1 = new Form1();
                form1.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               // Form1 form1 = new Form1();
               // form1.Show();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static SecureString GetSecureString(string str)
        {
            SecureString secureString = new SecureString();
            foreach (char ch in str)
            {
                secureString.AppendChar(ch);
            }
            secureString.MakeReadOnly();
            return secureString;
        }


        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}

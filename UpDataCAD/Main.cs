using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UpDataCAD
{
    public class SysTrayApp : Form
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        public static void Main()
        {
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 0);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.WriteLine("Test");
            Application.Run(new SysTrayApp());

        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public SysTrayApp()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();

            trayMenu.MenuItems.Add("Aktualizator internetowy CADDecor", OnUpadateCAD);
            trayMenu.MenuItems.Add("Aktualizacja z repozytorium", OnRepoUpdate);
            trayMenu.MenuItems.Add("Okno", Window);
            trayMenu.MenuItems.Add("Wypakowanie plików", ExtractFiles);
            trayMenu.MenuItems.Add("Sprawdź czy są nowe aktualizacje", IsUpdate);
            trayMenu.MenuItems.Add("Exit", OnExit);
            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

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

        private void IsUpdate(object sender, EventArgs e)
        {
            Download d = new Download();
            if (d.IsNewUpdate())
                MessageBox.Show("Są już dostępne nowe aktualizacje baz danych");
            else
                MessageBox.Show("Brak nowych aktualizacji");
        }

        private void OnRepoUpdate(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "AutoUpdataCaD.exe";

            p.Start();
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

        private void OnUpadateCAD(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "iUPDATE.EXE";

            p.Start();
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{
    public partial class WhoRunApp : Form
    {
        Thread tr;
        Who who;
        string token;
        string page;
        Dictionary<string, bool> isValidDict;

        public WhoRunApp()
        {
            isValidDict = new Dictionary<string, bool>(){
                { "department", false },
                { "tboxName", false},
                { "tboxLastName", false },
                { "email", false },
                { "phone", false }
            };
            

            page = "https://1sw.pl";
            token = ""; 

            InitializeComponent();
            foreach (var item in LoadJson())
            {
                comboBox1.Items.Add(item.department);
            }
            
        }

        

        private void btnOK_Click(object sender, EventArgs e)
        {
            //tboxPhone.mas
          
            who = new Who();
            who.department = comboBox1.Text;
            who.name = tboxName.Text;
            who.lastname = tboxLastName.Text;
            who.phone = maskTBoxPhone.Text; //masktBoxPhone.Text;
            who.email = tboxEmail.Text;

            if (IsAllValid())
            {
                this.Close();

                tr = new Thread(OpenMainWindow);
                tr.SetApartmentState(ApartmentState.STA);
                tr.Start();
                //Application.Run(new Form1(who));
            }
            else
            {
                MessageBox.Show("Proszę poprawić błędy w formularzu");
            }
        }

        private void OpenMainWindow()
        {
            Application.Run(new Form1(who));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
       
        bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Verfication(Who who)
        {
            return true;
        }

        private void tboxEmail_Leave(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (IsValidEmail(tboxEmail.Text))
            {
                txtBox.BackColor = Color.White;
                isValidDict["email"] = true;
            }
            else
            {
                txtBox.BackColor = Color.Red;
                isValidDict["email"] = false;
            }
        }

        private void tboxName_Leave(object sender, EventArgs e)
        {

        }

        private void tboxLastName_Leave(object sender, EventArgs e)
        {

        }

        private bool IsValid = false;

        private void SetProperty(Control ctr)
        {
            foreach (Control control in ctr.Controls)
            {
                if (control is TextBox)
                {
                    control.Validating += ValidateText;
                }
                else
                {
                    if (control.HasChildren)
                    {
                        SetProperty(control);  //Recursive function if the control is nested
                    }
                }
            }
        }

        private void VaildateDepartment(object sender, EventArgs e)
        {
            ComboBox txtBox = sender as ComboBox;
            
            if (txtBox.Text.Length > 0)
            {
                txtBox.BackColor = Color.White;
                isValidDict["department"] = true;
            }
            else
            {
                txtBox.BackColor = Color.Red;
                isValidDict["department"] = false;
            }
        }

        private void ValidatePhone(object sender, EventArgs e)
        {
            MaskedTextBox txtBox = sender as MaskedTextBox;
            string strTmp = Regex.Replace(txtBox.Text, @"\s", "");
            if (strTmp.Length == 12)
            {
                txtBox.BackColor = Color.White;
                isValidDict["phone"] = true;
            }
            else
            {
                txtBox.BackColor = Color.Red;
                isValidDict["phone"] = false;
            }
        }

        private void ValidateText(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            String strpattern = "[\\x20.A-Za-ząćęłńóśźżĄĘŁŃÓŚŹŻ]"; //Pattern is Ok
            Regex regex = new Regex(strpattern);
            //What should I write here?
            if (!regex.Match(txtBox.Text).Success || txtBox.Text.Length < 3)
            {
                txtBox.BackColor = Color.Red;
                isValidDict[txtBox.Name] = false;
            }
            else
            {
                txtBox.BackColor = Color.White;
                isValidDict[txtBox.Name] = true;
            }
        }

        /// <summary>
        /// Pobiera listę oddziałów z pliku department.json
        /// </summary>
        /// <returns>Zwraca listę oddziałów</returns>
        public List<Item> LoadJson()
        {
            List<Item> items;
            using (StreamReader r = new StreamReader("department.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }

            return items;
        }

        public class Item
        {
            public string department;        
        }

        /// <summary>
        /// Sprawdzanie całego formularza
        /// </summary>
        /// <returns></returns>
        private bool IsAllValid()
        {          
            foreach (var item in isValidDict)
            {
                if (!item.Value)
                    return false;
            }
            return true;
        }

        private void WhoRunApp_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Skrót klawiszowy Ctrl + F pozwalający ominąć okno Who
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                who = new Who();
                who.department = "Bytom";
                who.name = "Jan";
                who.lastname = "Testowy";
                who.phone = "666777555";
                who.email = "jan.testowy@tgs.pl";

               
                    this.Close();

                    tr = new Thread(OpenMainWindow);
                    tr.SetApartmentState(ApartmentState.STA);
                    tr.Start();
                   

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}


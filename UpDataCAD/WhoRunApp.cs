using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public WhoRunApp()
        {
            InitializeComponent();
            foreach (var item in LoadJson())
            {
                comboBox1.Items.Add(item.department);
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

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
            this.Close();
            
            tr = new Thread(OpenMainWindow);
            tr.SetApartmentState(ApartmentState.STA);
            tr.Start();
            //Application.Run(new Form1(who));
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
            if (IsValidEmail(tboxEmail.Text))
                IsValid = true;
            else
                IsValid = false;
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

        private void ValidateText(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            String strpattern = "[\\x20.a-zA-ZąćęłńóśźżĄĘŁŃÓŚŹŻ]"; //Pattern is Ok
            Regex regex = new Regex(strpattern);
            //What should I write here?
            if (!regex.Match(txtBox.Text).Success)
            {
                txtBox.BackColor = Color.Red;
                IsValid = true;
            }
            else
            {
                txtBox.BackColor = Color.White;
                IsValid = false;
            }
        }

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
    }
}

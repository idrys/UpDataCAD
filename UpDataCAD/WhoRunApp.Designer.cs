namespace UpDataCAD
{
    partial class WhoRunApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.tboxLastName = new System.Windows.Forms.TextBox();
            this.labPhone = new System.Windows.Forms.Label();
            this.tboxEmail = new System.Windows.Forms.TextBox();
            this.labEmail = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.maskTBoxPhone = new System.Windows.Forms.MaskedTextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Oddział";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(45, 60);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(200, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Imię";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(152, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nazwisko";
            // 
            // tboxName
            // 
            this.tboxName.Location = new System.Drawing.Point(45, 112);
            this.tboxName.Name = "tboxName";
            this.tboxName.Size = new System.Drawing.Size(100, 20);
            this.tboxName.TabIndex = 4;
            this.tboxName.Leave += new System.EventHandler(this.tboxName_Leave);
            this.tboxName.Validated += new System.EventHandler(this.ValidateText);
            // 
            // tboxLastName
            // 
            this.tboxLastName.Location = new System.Drawing.Point(145, 112);
            this.tboxLastName.Name = "tboxLastName";
            this.tboxLastName.Size = new System.Drawing.Size(100, 20);
            this.tboxLastName.TabIndex = 5;
            this.tboxLastName.Leave += new System.EventHandler(this.tboxLastName_Leave);
            this.tboxLastName.Validated += new System.EventHandler(this.ValidateText);
            // 
            // labPhone
            // 
            this.labPhone.AutoSize = true;
            this.labPhone.Location = new System.Drawing.Point(42, 154);
            this.labPhone.Name = "labPhone";
            this.labPhone.Size = new System.Drawing.Size(43, 13);
            this.labPhone.TabIndex = 6;
            this.labPhone.Text = "Telefon";
            // 
            // tboxEmail
            // 
            this.tboxEmail.Location = new System.Drawing.Point(151, 170);
            this.tboxEmail.Name = "tboxEmail";
            this.tboxEmail.Size = new System.Drawing.Size(94, 20);
            this.tboxEmail.TabIndex = 8;
            this.tboxEmail.Leave += new System.EventHandler(this.tboxEmail_Leave);
            this.tboxEmail.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            this.tboxEmail.Validated += new System.EventHandler(this.textBox1_Validated);
            // 
            // labEmail
            // 
            this.labEmail.AutoSize = true;
            this.labEmail.Location = new System.Drawing.Point(155, 154);
            this.labEmail.Name = "labEmail";
            this.labEmail.Size = new System.Drawing.Size(34, 13);
            this.labEmail.TabIndex = 9;
            this.labEmail.Text = "e-mail";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(170, 240);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(45, 240);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // maskTBoxPhone
            // 
            this.maskTBoxPhone.Location = new System.Drawing.Point(45, 170);
            this.maskTBoxPhone.Mask = "+48 000 000 000";
            this.maskTBoxPhone.Name = "maskTBoxPhone";
            this.maskTBoxPhone.Size = new System.Drawing.Size(100, 20);
            this.maskTBoxPhone.TabIndex = 12;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // WhoRunApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 300);
            this.Controls.Add(this.maskTBoxPhone);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labEmail);
            this.Controls.Add(this.tboxEmail);
            this.Controls.Add(this.labPhone);
            this.Controls.Add(this.tboxLastName);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Name = "WhoRunApp";
            this.Text = "Uruchamia aplikacje: ";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.TextBox tboxLastName;
        private System.Windows.Forms.Label labPhone;
        private System.Windows.Forms.TextBox tboxEmail;
        private System.Windows.Forms.Label labEmail;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MaskedTextBox maskTBoxPhone;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
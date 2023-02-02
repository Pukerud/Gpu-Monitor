using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Button
{
    public partial class SettingsForm : Form
    {
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string MailTo { get; set; }
        public string MailFrom { get; set; }
        public int EmailTimeout { get; set; }
        public int CheckTimer { get; set; }
        public int DobbelCheck { get; set; }
        public string GsheetShare { get; set; }
        public int GsheetTimer { get; set; }

        public SettingsForm()
        {
            InitializeComponent();
        }
        
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {            
            SmtpServer = txtSmtpServer.Text;           
            SmtpUser = txtSmtpUser.Text;
            SmtpPassword = txtSmtpPassword.Text;
            MailTo = txtMailTo.Text;
            MailFrom = txtMailFrom.Text;
            GsheetShare = txtGsheetShare.Text;
            //GsheetTimer = txtGsheetTimer.Text;
            try
            {
                if (!string.IsNullOrEmpty(txtSmtpServerPort.Text))
                {
                    SmtpServerPort = int.Parse(txtSmtpServerPort.Text);
                }
                if (!string.IsNullOrEmpty(txtEmailTimeout.Text))
                {
                    EmailTimeout = int.Parse(txtEmailTimeout.Text);
                }
                if (!string.IsNullOrEmpty(txtCheckTimer.Text))
                {
                    CheckTimer = int.Parse(txtCheckTimer.Text);
                }
                if (!string.IsNullOrEmpty(txtDobbelCheck.Text))
                {
                    DobbelCheck = int.Parse(txtDobbelCheck.Text);
                }
                if (!string.IsNullOrEmpty(txtGsheetTimer.Text))
                {
                    GsheetTimer = int.Parse(txtGsheetTimer.Text);
                }
            }
            catch (FormatException ex)
            {
                // Handle the exception, for example by displaying an error message                
                ErrorForm errorForm = new ErrorForm(ex.Message);
                errorForm.ShowDialog();
                return;
            }
            // Save the settings to a file or in the registry
            // ...
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\GPUMonitor");
            key.SetValue("GsheetShare", GsheetShare);
            key.SetValue("SmtpServer", SmtpServer);
            key.SetValue("SmtpServerPort", SmtpServerPort);
            key.SetValue("SmtpUser", SmtpUser);
            key.SetValue("SmtpPassword", SmtpPassword);
            key.SetValue("MailTo", MailTo);
            key.SetValue("MailFrom", MailFrom);
            key.SetValue("EmailTimeout", EmailTimeout);
            key.SetValue("CheckTimer", CheckTimer);
            key.SetValue("DobbelCheck", DobbelCheck);
            key.SetValue("GsheetTimer", GsheetTimer);

            this.Close();
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }       
        private void SettingsForm_Load(object sender, EventArgs e)
        {

            
        }

        private void txtSmtpServer_Click(object sender, EventArgs e)
        {

        }

        private void SettingsForm_Load_1(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\GPUMonitor"))
            {
                if (key != null)
                {
                    txtSmtpServer.Text = SmtpServer;
                    txtSmtpServerPort.Text = SmtpServerPort.ToString();
                    txtSmtpUser.Text = SmtpUser;
                    txtSmtpPassword.Text = SmtpPassword;
                    txtMailTo.Text = MailTo;
                    txtMailFrom.Text = MailFrom;
                    txtGsheetShare.Text = GsheetShare;
                    //txtGsheetTimer.Text = GsheetTimer.ToString();
                    txtSmtpServer.Text = key.GetValue("SmtpServer").ToString();
                    txtSmtpServerPort.Text = key.GetValue("SmtpServerPort").ToString();
                    txtSmtpUser.Text = key.GetValue("SmtpUser").ToString();
                    txtSmtpPassword.Text = key.GetValue("SmtpPassword").ToString();
                    txtMailTo.Text = key.GetValue("MailTo").ToString();
                    txtMailFrom.Text = key.GetValue("MailFrom").ToString();
                    txtEmailTimeout.Text = key.GetValue("EmailTimeout").ToString();
                    txtCheckTimer.Text = key.GetValue("CheckTimer").ToString();
                    txtDobbelCheck.Text = key.GetValue("DobbelCheck").ToString();
                    txtGsheetShare.Text = key.GetValue("GsheetShare").ToString();
                    txtGsheetTimer.Text = key.GetValue("GsheetTimer").ToString();
                }
            }
        }
       
        private void txtSmtpServer_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtMailFrom_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtGsheetTimer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

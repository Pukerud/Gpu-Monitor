namespace Simple_Button
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.SaveForm = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtSmtpServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSmtpServerPort = new System.Windows.Forms.TextBox();
            this.txtSmtpUser = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSmtpPassword = new System.Windows.Forms.TextBox();
            this.txtMailTo = new System.Windows.Forms.TextBox();
            this.txtMailFrom = new System.Windows.Forms.TextBox();
            this.txtEmailTimeout = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCheckTimer = new System.Windows.Forms.TextBox();
            this.txtDobbelCheck = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtGsheetShare = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtGsheetTimer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SaveForm
            // 
            this.SaveForm.Location = new System.Drawing.Point(330, 156);
            this.SaveForm.Name = "SaveForm";
            this.SaveForm.Size = new System.Drawing.Size(75, 23);
            this.SaveForm.TabIndex = 0;
            this.SaveForm.Text = "Save";
            this.SaveForm.UseVisualStyleBackColor = true;
            this.SaveForm.Click += new System.EventHandler(this.button1_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Label1.Location = new System.Drawing.Point(12, 20);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(67, 15);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "SmtpServer";
            this.Label1.Click += new System.EventHandler(this.txtSmtpServer_Click);
            // 
            // txtSmtpServer
            // 
            this.txtSmtpServer.Location = new System.Drawing.Point(114, 12);
            this.txtSmtpServer.Name = "txtSmtpServer";
            this.txtSmtpServer.Size = new System.Drawing.Size(190, 23);
            this.txtSmtpServer.TabIndex = 2;
            this.txtSmtpServer.TextChanged += new System.EventHandler(this.txtSmtpServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "SmtpServerPort";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtSmtpServerPort
            // 
            this.txtSmtpServerPort.Location = new System.Drawing.Point(114, 41);
            this.txtSmtpServerPort.Name = "txtSmtpServerPort";
            this.txtSmtpServerPort.Size = new System.Drawing.Size(190, 23);
            this.txtSmtpServerPort.TabIndex = 4;
            // 
            // txtSmtpUser
            // 
            this.txtSmtpUser.Location = new System.Drawing.Point(114, 70);
            this.txtSmtpUser.Name = "txtSmtpUser";
            this.txtSmtpUser.Size = new System.Drawing.Size(190, 23);
            this.txtSmtpUser.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "SmtpUser";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "SmtpPassword";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(12, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "MailTo";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(12, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "MailFrom";
            // 
            // txtSmtpPassword
            // 
            this.txtSmtpPassword.Location = new System.Drawing.Point(114, 99);
            this.txtSmtpPassword.Name = "txtSmtpPassword";
            this.txtSmtpPassword.Size = new System.Drawing.Size(190, 23);
            this.txtSmtpPassword.TabIndex = 10;
            this.txtSmtpPassword.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // txtMailTo
            // 
            this.txtMailTo.Location = new System.Drawing.Point(114, 128);
            this.txtMailTo.Name = "txtMailTo";
            this.txtMailTo.Size = new System.Drawing.Size(190, 23);
            this.txtMailTo.TabIndex = 11;
            this.txtMailTo.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // txtMailFrom
            // 
            this.txtMailFrom.Location = new System.Drawing.Point(114, 157);
            this.txtMailFrom.Name = "txtMailFrom";
            this.txtMailFrom.Size = new System.Drawing.Size(190, 23);
            this.txtMailFrom.TabIndex = 12;
            this.txtMailFrom.TextChanged += new System.EventHandler(this.txtMailFrom_TextChanged);
            // 
            // txtEmailTimeout
            // 
            this.txtEmailTimeout.Location = new System.Drawing.Point(456, 12);
            this.txtEmailTimeout.Name = "txtEmailTimeout";
            this.txtEmailTimeout.PlaceholderText = "In Minuts";
            this.txtEmailTimeout.Size = new System.Drawing.Size(81, 23);
            this.txtEmailTimeout.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label7.Location = new System.Drawing.Point(328, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Email Timeout";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label8.Location = new System.Drawing.Point(328, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 15);
            this.label8.TabIndex = 15;
            this.label8.Text = "Check Timer";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label9.Location = new System.Drawing.Point(328, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(122, 15);
            this.label9.TabIndex = 16;
            this.label9.Text = "Dobbel Check Timout";
            // 
            // txtCheckTimer
            // 
            this.txtCheckTimer.Location = new System.Drawing.Point(456, 41);
            this.txtCheckTimer.Name = "txtCheckTimer";
            this.txtCheckTimer.PlaceholderText = "In Seconds";
            this.txtCheckTimer.Size = new System.Drawing.Size(81, 23);
            this.txtCheckTimer.TabIndex = 17;
            // 
            // txtDobbelCheck
            // 
            this.txtDobbelCheck.Location = new System.Drawing.Point(456, 70);
            this.txtDobbelCheck.Name = "txtDobbelCheck";
            this.txtDobbelCheck.PlaceholderText = "In Seconds";
            this.txtDobbelCheck.Size = new System.Drawing.Size(81, 23);
            this.txtDobbelCheck.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label10.Location = new System.Drawing.Point(12, 194);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "RnDr Log Email";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // txtGsheetShare
            // 
            this.txtGsheetShare.Location = new System.Drawing.Point(114, 186);
            this.txtGsheetShare.Name = "txtGsheetShare";
            this.txtGsheetShare.PlaceholderText = "Sheet is shared with this Email";
            this.txtGsheetShare.Size = new System.Drawing.Size(190, 23);
            this.txtGsheetShare.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label11.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label11.Location = new System.Drawing.Point(328, 107);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 15);
            this.label11.TabIndex = 21;
            this.label11.Text = "Gsheet Timer";
            // 
            // txtGsheetTimer
            // 
            this.txtGsheetTimer.Location = new System.Drawing.Point(456, 99);
            this.txtGsheetTimer.Name = "txtGsheetTimer";
            this.txtGsheetTimer.PlaceholderText = "In Minuts";
            this.txtGsheetTimer.Size = new System.Drawing.Size(81, 23);
            this.txtGsheetTimer.TabIndex = 22;
            this.txtGsheetTimer.TextChanged += new System.EventHandler(this.txtGsheetTimer_TextChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(557, 220);
            this.Controls.Add(this.txtGsheetTimer);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtGsheetShare);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtDobbelCheck);
            this.Controls.Add(this.txtCheckTimer);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtEmailTimeout);
            this.Controls.Add(this.txtMailFrom);
            this.Controls.Add(this.txtMailTo);
            this.Controls.Add(this.txtSmtpPassword);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSmtpUser);
            this.Controls.Add(this.txtSmtpServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSmtpServer);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.SaveForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button SaveForm;
        private Label Label1;
        private TextBox txtSmtpServer;
        private Label label2;
        private TextBox txtSmtpServerPort;
        private TextBox txtSmtpUser;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtSmtpPassword;
        private TextBox txtMailTo;
        private TextBox txtMailFrom;
        private TextBox txtEmailTimeout;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox txtCheckTimer;
        private TextBox txtDobbelCheck;
        private Label label10;
        private TextBox txtGsheetShare;
        private Label label11;
        private TextBox txtGsheetTimer;
    }
}
namespace SshTunnel
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this._btnDisconnect = new System.Windows.Forms.Button();
            this._btnConnect = new System.Windows.Forms.Button();
            this._txtUserPswd = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this._txtUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this._btnClearLog = new System.Windows.Forms.Button();
            this._lstLogs = new System.Windows.Forms.ListBox();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = "DB Server";
            this.groupBox5.Controls.Add(this._btnDisconnect);
            this.groupBox5.Controls.Add(this._btnConnect);
            this.groupBox5.Controls.Add(this._txtUserPswd);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this._txtUserName);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(397, 102);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Authenticaton";
            // 
            // _btnDisconnect
            // 
            this._btnDisconnect.Location = new System.Drawing.Point(296, 30);
            this._btnDisconnect.Name = "_btnDisconnect";
            this._btnDisconnect.Size = new System.Drawing.Size(87, 48);
            this._btnDisconnect.TabIndex = 3;
            this._btnDisconnect.Text = "Disconnect";
            this._btnDisconnect.UseVisualStyleBackColor = true;
            this._btnDisconnect.Click += new System.EventHandler(this._btnDisconnect_Click);
            // 
            // _btnConnect
            // 
            this._btnConnect.Location = new System.Drawing.Point(203, 30);
            this._btnConnect.Name = "_btnConnect";
            this._btnConnect.Size = new System.Drawing.Size(87, 48);
            this._btnConnect.TabIndex = 2;
            this._btnConnect.Text = "Connect";
            this._btnConnect.UseVisualStyleBackColor = true;
            this._btnConnect.Click += new System.EventHandler(this._btnConnect_Click);
            // 
            // _txtUserPswd
            // 
            this._txtUserPswd.Location = new System.Drawing.Point(76, 58);
            this._txtUserPswd.Name = "_txtUserPswd";
            this._txtUserPswd.PasswordChar = '*';
            this._txtUserPswd.Size = new System.Drawing.Size(106, 20);
            this._txtUserPswd.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Password";
            // 
            // _txtUserName
            // 
            this._txtUserName.Location = new System.Drawing.Point(76, 32);
            this._txtUserName.Name = "_txtUserName";
            this._txtUserName.Size = new System.Drawing.Size(106, 20);
            this._txtUserName.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "User Name";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this._btnClearLog);
            this.groupBox6.Controls.Add(this._lstLogs);
            this.groupBox6.Location = new System.Drawing.Point(12, 135);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(397, 220);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Log";
            // 
            // _btnClearLog
            // 
            this._btnClearLog.Location = new System.Drawing.Point(304, 11);
            this._btnClearLog.Name = "_btnClearLog";
            this._btnClearLog.Size = new System.Drawing.Size(87, 21);
            this._btnClearLog.TabIndex = 0;
            this._btnClearLog.Text = "Clear";
            this._btnClearLog.UseVisualStyleBackColor = true;
            this._btnClearLog.Click += new System.EventHandler(this._btnClearLog_Click);
            // 
            // _lstLogs
            // 
            this._lstLogs.FormattingEnabled = true;
            this._lstLogs.Location = new System.Drawing.Point(9, 36);
            this._lstLogs.Name = "_lstLogs";
            this._lstLogs.Size = new System.Drawing.Size(382, 173);
            this._lstLogs.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 368);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "AppOnline";
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox _txtUserPswd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox _txtUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button _btnConnect;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button _btnClearLog;
        private System.Windows.Forms.ListBox _lstLogs;
        private System.Windows.Forms.Button _btnDisconnect;
    }
}


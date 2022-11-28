namespace MeetingsSync
{
    partial class TestForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.btnLogDelete = new System.Windows.Forms.Button();
            this.txtWriteToLog = new System.Windows.Forms.TextBox();
            this.btnLogClear = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnReadLog = new System.Windows.Forms.Button();
            this.btnWriteToLog = new System.Windows.Forms.Button();
            this.btnNewLog = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.txtSettings = new System.Windows.Forms.TextBox();
            this.btnReadSettings = new System.Windows.Forms.Button();
            this.tabGetEmails = new System.Windows.Forms.TabPage();
            this.txtGetEmails = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnGetEmails = new System.Windows.Forms.Button();
            this.tabGetCalendarInfo = new System.Windows.Forms.TabPage();
            this.btnGetCalendarInfo = new System.Windows.Forms.Button();
            this.txtCalendarInfo = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabGetEmails.SuspendLayout();
            this.tabGetCalendarInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabGetEmails);
            this.tabControl1.Controls.Add(this.tabGetCalendarInfo);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1016, 567);
            this.tabControl1.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.btnLogDelete);
            this.tabLog.Controls.Add(this.txtWriteToLog);
            this.tabLog.Controls.Add(this.btnLogClear);
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Controls.Add(this.btnReadLog);
            this.tabLog.Controls.Add(this.btnWriteToLog);
            this.tabLog.Controls.Add(this.btnNewLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(1008, 541);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // btnLogDelete
            // 
            this.btnLogDelete.Location = new System.Drawing.Point(927, 6);
            this.btnLogDelete.Name = "btnLogDelete";
            this.btnLogDelete.Size = new System.Drawing.Size(75, 23);
            this.btnLogDelete.TabIndex = 6;
            this.btnLogDelete.Text = "Delete Log";
            this.btnLogDelete.UseVisualStyleBackColor = true;
            // 
            // txtWriteToLog
            // 
            this.txtWriteToLog.Location = new System.Drawing.Point(87, 37);
            this.txtWriteToLog.Name = "txtWriteToLog";
            this.txtWriteToLog.Size = new System.Drawing.Size(371, 20);
            this.txtWriteToLog.TabIndex = 5;
            // 
            // btnLogClear
            // 
            this.btnLogClear.Location = new System.Drawing.Point(927, 34);
            this.btnLogClear.Name = "btnLogClear";
            this.btnLogClear.Size = new System.Drawing.Size(75, 23);
            this.btnLogClear.TabIndex = 4;
            this.btnLogClear.Text = "Clear Log";
            this.btnLogClear.UseVisualStyleBackColor = true;
            this.btnLogClear.Click += new System.EventHandler(this.btnLogClear_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(6, 93);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(996, 442);
            this.txtLog.TabIndex = 3;
            // 
            // btnReadLog
            // 
            this.btnReadLog.Location = new System.Drawing.Point(6, 64);
            this.btnReadLog.Name = "btnReadLog";
            this.btnReadLog.Size = new System.Drawing.Size(75, 23);
            this.btnReadLog.TabIndex = 2;
            this.btnReadLog.Text = "Read Log";
            this.btnReadLog.UseVisualStyleBackColor = true;
            this.btnReadLog.Click += new System.EventHandler(this.btnReadLog_Click);
            // 
            // btnWriteToLog
            // 
            this.btnWriteToLog.Location = new System.Drawing.Point(6, 35);
            this.btnWriteToLog.Name = "btnWriteToLog";
            this.btnWriteToLog.Size = new System.Drawing.Size(75, 23);
            this.btnWriteToLog.TabIndex = 1;
            this.btnWriteToLog.Text = "WriteToLog";
            this.btnWriteToLog.UseVisualStyleBackColor = true;
            this.btnWriteToLog.Click += new System.EventHandler(this.btnWriteToLog_Click);
            // 
            // btnNewLog
            // 
            this.btnNewLog.Enabled = false;
            this.btnNewLog.Location = new System.Drawing.Point(6, 6);
            this.btnNewLog.Name = "btnNewLog";
            this.btnNewLog.Size = new System.Drawing.Size(75, 23);
            this.btnNewLog.TabIndex = 0;
            this.btnNewLog.Text = "New Log";
            this.btnNewLog.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.txtSettings);
            this.tabSettings.Controls.Add(this.btnReadSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(1008, 541);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // txtSettings
            // 
            this.txtSettings.Location = new System.Drawing.Point(6, 35);
            this.txtSettings.Multiline = true;
            this.txtSettings.Name = "txtSettings";
            this.txtSettings.Size = new System.Drawing.Size(988, 489);
            this.txtSettings.TabIndex = 1;
            // 
            // btnReadSettings
            // 
            this.btnReadSettings.Location = new System.Drawing.Point(6, 6);
            this.btnReadSettings.Name = "btnReadSettings";
            this.btnReadSettings.Size = new System.Drawing.Size(75, 23);
            this.btnReadSettings.TabIndex = 0;
            this.btnReadSettings.Text = "Read";
            this.btnReadSettings.UseVisualStyleBackColor = true;
            this.btnReadSettings.Click += new System.EventHandler(this.btnReadSettings_Click);
            // 
            // tabGetEmails
            // 
            this.tabGetEmails.Controls.Add(this.txtGetEmails);
            this.tabGetEmails.Controls.Add(this.lblPath);
            this.tabGetEmails.Controls.Add(this.btnGetEmails);
            this.tabGetEmails.Location = new System.Drawing.Point(4, 22);
            this.tabGetEmails.Name = "tabGetEmails";
            this.tabGetEmails.Padding = new System.Windows.Forms.Padding(3);
            this.tabGetEmails.Size = new System.Drawing.Size(1008, 541);
            this.tabGetEmails.TabIndex = 1;
            this.tabGetEmails.Text = "GetEmails";
            this.tabGetEmails.UseVisualStyleBackColor = true;
            // 
            // txtGetEmails
            // 
            this.txtGetEmails.Location = new System.Drawing.Point(9, 48);
            this.txtGetEmails.Multiline = true;
            this.txtGetEmails.Name = "txtGetEmails";
            this.txtGetEmails.Size = new System.Drawing.Size(993, 487);
            this.txtGetEmails.TabIndex = 2;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(6, 32);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(0, 13);
            this.lblPath.TabIndex = 1;
            // 
            // btnGetEmails
            // 
            this.btnGetEmails.Location = new System.Drawing.Point(6, 6);
            this.btnGetEmails.Name = "btnGetEmails";
            this.btnGetEmails.Size = new System.Drawing.Size(75, 23);
            this.btnGetEmails.TabIndex = 0;
            this.btnGetEmails.Text = "Hae";
            this.btnGetEmails.UseVisualStyleBackColor = true;
            this.btnGetEmails.Click += new System.EventHandler(this.btnGetEmails_Click);
            // 
            // tabGetCalendarInfo
            // 
            this.tabGetCalendarInfo.Controls.Add(this.btnGetCalendarInfo);
            this.tabGetCalendarInfo.Controls.Add(this.txtCalendarInfo);
            this.tabGetCalendarInfo.Location = new System.Drawing.Point(4, 22);
            this.tabGetCalendarInfo.Name = "tabGetCalendarInfo";
            this.tabGetCalendarInfo.Size = new System.Drawing.Size(1008, 541);
            this.tabGetCalendarInfo.TabIndex = 2;
            this.tabGetCalendarInfo.Text = "GetCalendarInfo";
            this.tabGetCalendarInfo.UseVisualStyleBackColor = true;
            // 
            // btnGetCalendarInfo
            // 
            this.btnGetCalendarInfo.Location = new System.Drawing.Point(6, 6);
            this.btnGetCalendarInfo.Name = "btnGetCalendarInfo";
            this.btnGetCalendarInfo.Size = new System.Drawing.Size(75, 23);
            this.btnGetCalendarInfo.TabIndex = 1;
            this.btnGetCalendarInfo.Text = "Hae";
            this.btnGetCalendarInfo.UseVisualStyleBackColor = true;
            this.btnGetCalendarInfo.Click += new System.EventHandler(this.btnGetCalendarInfo_Click);
            // 
            // txtCalendarInfo
            // 
            this.txtCalendarInfo.Location = new System.Drawing.Point(6, 35);
            this.txtCalendarInfo.Multiline = true;
            this.txtCalendarInfo.Name = "txtCalendarInfo";
            this.txtCalendarInfo.Size = new System.Drawing.Size(986, 491);
            this.txtCalendarInfo.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 591);
            this.Controls.Add(this.tabControl1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.tabControl1.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.tabGetEmails.ResumeLayout(false);
            this.tabGetEmails.PerformLayout();
            this.tabGetCalendarInfo.ResumeLayout(false);
            this.tabGetCalendarInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabGetEmails;
        private System.Windows.Forms.TabPage tabGetCalendarInfo;
        private System.Windows.Forms.Button btnReadLog;
        private System.Windows.Forms.Button btnWriteToLog;
        private System.Windows.Forms.Button btnNewLog;
        private System.Windows.Forms.Button btnLogClear;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtWriteToLog;
        private System.Windows.Forms.Button btnLogDelete;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TextBox txtSettings;
        private System.Windows.Forms.Button btnReadSettings;
        private System.Windows.Forms.TextBox txtGetEmails;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button btnGetEmails;
        private System.Windows.Forms.Button btnGetCalendarInfo;
        private System.Windows.Forms.TextBox txtCalendarInfo;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeetingsSync
{
    public partial class TestForm : Form
    {
        //private Log log;
        private Settings settings;
        private List<string> emails;

        public TestForm()
        {
            InitializeComponent();

            //this.log = Log.Instance;
        }

        private void btnLogClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void btnWriteToLog_Click(object sender, EventArgs e)
        {
            //log.WriteInformation(Log.Level.Debug, txtWriteToLog.Text);
            //Log.TestWriteToLog(txtWriteToLog.Text);
        }

        private void btnReadLog_Click(object sender, EventArgs e)
        {
            //txtLog.Text = log.ReadLog();
        }

        private void btnReadSettings_Click(object sender, EventArgs e)
        {
            settings = new Settings();
            txtSettings.Text = string.Concat(txtSettings.Text, "emailListPath = ", settings.EmailListPath, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "calendarEmailuser = ", settings.CalendarEmailUser, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "calendarDataPath = ", settings.CalendarDataPath, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "exchangeUserName = ", settings.ExchangeUserName, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "exchangeUserPwd = ", settings.ExchangeUserPwd, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "calendarPostApiKey = ", settings.CalendarPostApiKey, Environment.NewLine);
            txtSettings.Text = string.Concat(txtSettings.Text, "calendarPostSecret = ", settings.CalendarPostApiSecret, Environment.NewLine);
        }

        private void btnGetEmails_Click(object sender, EventArgs e)
        {
            lblPath.Text = settings.EmailListPath;
            EmailList getEmails = new EmailList(settings.EmailListPath);
            emails = getEmails.GetEmail();

            foreach (string email in emails)
            {
                txtGetEmails.Text = string.Concat(txtGetEmails.Text, email, Environment.NewLine);
            }
        }

        private void btnGetCalendarInfo_Click(object sender, EventArgs e)
        {
            CalendarInfo calInfo = new CalendarInfo(settings);
            CalendarItemJson itemJson = calInfo.getCalendarInfo(settings.CalendarEmailUser, DateTime.Now, DateTime.Now.AddDays(90));
            CalendarInfo.PostCalendarInfo(itemJson, settings.CalendarDataPath);
        }
    }
}

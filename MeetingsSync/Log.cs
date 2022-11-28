//#define EVENTLOG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace MeetingsSync
{
    class Log
    {
        //public enum Level
        //{
        //    Debug,
        //    Release
        //}

        public static string failedLogs = String.Empty;

        private static Log instance;
        private static Object locker = new Object();

        private readonly string EVENT_LOG_SOURCE = "SyncService";
        private readonly string EVENT_LOG_LOGNAME = "MeetingsLogs";
        private EventLog eventLog;

        private string logFilePath = @"C:\MeetingsSync\Log.txt";

        private Settings settings;

        private Log()
        {
            settings = new Settings();
            if (settings.UseEventLogLogging)
            {
                this.eventLog = new EventLog();
                this.eventLog.Source = this.EVENT_LOG_SOURCE;
                this.eventLog.Log = this.EVENT_LOG_LOGNAME;
            }
        }

        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
        }

        public void WriteError(string message)
        {
            this.WriteToLog(EventLogEntryType.Error, message);
        }

        public void WriteWarning(string message)
        {
            this.WriteToLog(EventLogEntryType.Warning, message);
        }

        public void WriteInformation(string message)
        {
            this.WriteToLog(EventLogEntryType.Information, message);
        }

        private void WriteToLog(EventLogEntryType type, string message)
        {
            lock (locker)
            {
                try
                {
                    string lineWithDate = String.Format("{0:s}", DateTime.Now) + " " + message;

                    if (settings.UseConsoleLogging)
                    {
                        System.Console.WriteLine(lineWithDate);
                    }

                    if (settings.UseEventLogLogging)
                    {
                        this.eventLog.WriteEntry(message, type);
                    }
                    if (settings.UseFileLogging)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath, true))
                        {
                            file.WriteLine( lineWithDate );
                        }
                    }
                    if (settings.UseRemoteLogging)
                    {
                        try
                        {
                            LogJson json = new LogJson(settings.CalendarPostApiKey, settings.CalendarPostApiSecret, lineWithDate );
                            string postData = JsonConvert.SerializeObject(json);

                            WebRequest request = WebRequest.Create(settings.LogPath);
                            request.Method = "POST";

                            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                            request.ContentType = "application/json";
                            request.ContentLength = byteArray.Length;
                            Stream dataStream = request.GetRequestStream();
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            dataStream.Close();

                            WebResponse response = request.GetResponse();
                            dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            reader.ReadToEnd();
                            reader.Close();
                            dataStream.Close();
                            response.Close();
                        }
                        catch
                        {
                            // Do nothing if remote logging fails
                        }
                    }

                }
                catch (Exception e)
                {
                    failedLogs = failedLogs + DateTime.Now.ToString() + " " + message + " (" + e.ToString() + ")" + Environment.NewLine;
                }
            }
        }
    }

    class LogJson
    {
        public string api_key = string.Empty;
        public string api_secret = string.Empty;
        public string log = string.Empty;

        public LogJson(string apiKey, string apiSecret, string logString)
        {
            api_key = apiKey;
            api_secret = apiSecret;
            log = logString;
        }
    }
}

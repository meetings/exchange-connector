using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MeetingsSync
{
    class SettingsResult
    {
        [JsonProperty("refresh_key")]
        public string RefreshKey { get; set; }

        [JsonProperty("users")]
        public List<string> Emails { get; set; }
    }
    class EmailList
    {
        private string path;
        private SettingsResult result;

        public EmailList(string path)
        {
            this.path = path;
        }

        public List<string> GetEmail()
        {
            if( result == null ) fetchData();
            return result.Emails;
        }
        public string GetRefreshKey()
        {
            if (result == null) fetchData();
            return result.RefreshKey;
        }
        private void fetchData()
        {

            string responseStatus;

            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseStatus = response.StatusDescription;
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string json = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                result = JsonConvert.DeserializeObject<SettingsResult>(json);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Error while reading emails from Meetin.gs server: ", ex.Message, " FROM ", ex.InnerException ) );
            }
        }
    }
}

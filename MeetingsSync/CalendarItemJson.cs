using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingsSync
{
    class CalendarItemJson
    {
        public string api_key = string.Empty;
        public string api_secret = string.Empty;
        public string email = string.Empty;
        public string calendar_id = string.Empty;
        public string calendar_name = string.Empty;
        public bool calendar_is_primary = true;
        public int timespan_begin_epoch = 0;
        public int timespan_end_epoch = 0;
        public List<Entries> entries;

        public DateTime TimespanBeginEpoch
        { 
            set
            {
                this.timespan_begin_epoch = CalendarItemJson.ConvertTimeStampToEpoch(value);
            }
        }

        public DateTime TimespanEndEpoch
        {
            set
            {
                this.timespan_end_epoch = CalendarItemJson.ConvertTimeStampToEpoch(value);
            }
        }

        private static int ConvertTimeStampToEpoch(DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32((dateTime.ToUniversalTime() - epoch).TotalSeconds);
        }

        public CalendarItemJson(string apiKey, string apiSecret)
        {
            api_key = apiKey;
            api_secret = apiSecret;
            this.entries = new List<Entries>();
        }

        public bool HasEntries()
        {
            return entries.Count() > 0 ? true : false;
        }
    }

    class Entries
    {
        public string uid = string.Empty;
        public string title = string.Empty;
        public int begin_epoch = 0;
        public int end_epoch = 0;
        public string freebusy_description = string.Empty;
        public string freebusy_value = string.Empty;
        public string description = string.Empty;
        public string location = string.Empty;
        public List<string> participant_list;

        public string FreeBusyStatus
        {
            set
            {
                this.freebusy_description = value;
                this.freebusy_value = (value == "Free") ? "free" : "busy";
            }
        }
        public DateTime BeginEpoch
        {
            set
            {
                this.begin_epoch = Entries.ConvertTimeStampToEpoch(value);
            }
        }

        public DateTime EndEpoch
        {
            set
            {
                this.end_epoch = Entries.ConvertTimeStampToEpoch(value);
            }
        }

        private static int ConvertTimeStampToEpoch(DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32((dateTime.ToUniversalTime() - epoch).TotalSeconds);
        }

        public Entries()
        {
            this.participant_list = new List<string>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingsSync
{
	public class Settings
	{


		private TimeSpan dailyGetAllTimeStart;
		private TimeSpan dailyGetAllTimeEnd;

#if DEBUG
		private bool runForegroundTest = false;
        private bool useConsoleLoggingTest = false;
		private bool useFileLoggingTest = false;
		private bool useRemoteLoggingTest = false;
		private bool useEventLogLoggingTest = false;
		private string emailListPathTest;
		private string calendarEmailUserTest;
		private string calendarDataPathTest;
		private string logPathTest;
		private string exchangeUserNameTest;
		private string exchangeUserPwdTest;
		private string calendarPostApiKeyTest;
		private string calendarPostApiSecretTest;
		private bool skipCertificateValidationTest = false;
		private bool useHardCodedExchangeAddressTest = false;
		private string hardCodedExchangeAddressTest = string.Empty;
		private int queueProcessorConcurrencyTest;
        private int autodiscoverQueueProcessorConcurrencyTest;
        private int streamingSubscriptionGroupConcurrencyTest;
#else
		private bool runForeground = false;
        private bool useConsoleLogging = false;
		private bool useFileLogging = false;
		private bool useRemoteLogging = false;
		private bool useEventLogLogging = false;
		private string emailListPath;
		private string calendarEmailUser;
		private string calendarDataPath;
		private string logPath;
		private string exchangeUserName;
		private string exchangeUserPwd;
		private string calendarPostApiKey;
		private string calendarPostApiSecret;
		private bool skipCertificateValidation = false;
		private bool useHardCodedExchangeAddress = false;
		private string hardCodedExchangeAddress = string.Empty;
		private int queueProcessorConcurrency;
        private int autodiscoverQueueProcessorConcurrency;
        private int streamingSubscriptionGroupConcurrency;
#endif

		public bool RunForeground
		{
			get
			{
#if DEBUG
				return runForegroundTest;
#else
                return runForeground;
#endif
			}
		}

		public string EmailListPath
		{
			get
			{
#if DEBUG
				return emailListPathTest;
#else
                return emailListPath;
#endif
			}
		}

		public string CalendarEmailUser
		{
			get
			{
#if DEBUG
				return calendarEmailUserTest;
#else
                return calendarEmailUser;
#endif
			}
		}

		public string CalendarDataPath
		{
			get
			{
#if DEBUG
				return calendarDataPathTest;
#else
                return calendarDataPath;
#endif
			}
		}

		public string LogPath
		{
			get
			{
#if DEBUG
				return logPathTest;
#else
                return logPath;
#endif
			}
		}

		public TimeSpan DailyGetAllTimeStart
		{
			get
			{
				return dailyGetAllTimeStart;
			}
		}

		public TimeSpan DailyGetAllTimeEnd
		{
			get
			{
				return dailyGetAllTimeEnd;
			}
		}

		public string ExchangeUserName
		{
			get
			{
#if DEBUG
				return exchangeUserNameTest;
#else
                return exchangeUserName;
#endif
			}
		}

		public string ExchangeUserPwd
		{
			get
			{
#if DEBUG
				return exchangeUserPwdTest;
#else
                return exchangeUserPwd;
#endif
			}
		}

		public string CalendarPostApiKey
		{
			get
			{
#if DEBUG
				return calendarPostApiKeyTest;
#else
                return calendarPostApiKey;
#endif
			}
		}

		public string CalendarPostApiSecret
		{
			get
			{
#if DEBUG
				return calendarPostApiSecretTest;
#else
                return calendarPostApiSecret;
#endif
			}
		}

		public bool SkipCertificateValidation
		{
			get
			{
#if DEBUG
				return skipCertificateValidationTest;
#else
                return skipCertificateValidation;
#endif
			}
		}
		public bool UseHardCodedExchangeAddress
		{
			get
			{
#if DEBUG
				return useHardCodedExchangeAddressTest;
#else
                return useHardCodedExchangeAddress;
#endif
			}
		}

		public string HardCodedExchangeAddress
		{
			get
			{
#if DEBUG
				return hardCodedExchangeAddressTest;
#else
                return hardCodedExchangeAddress;
#endif
			}
		}
		public int QueueProcessorConcurrency
		{
			get
			{
#if DEBUG
				return queueProcessorConcurrencyTest;
#else
                return queueProcessorConcurrency;
#endif
			}
		}
        public int AutodiscoverQueueProcessorConcurrency
        {
            get
            {
#if DEBUG
                return autodiscoverQueueProcessorConcurrencyTest;
#else
                return autodiscoverQueueProcessorConcurrency;
#endif
            }
        }
        public int StreamingSubscriptionGroupConcurrency
        {
            get
            {
#if DEBUG
                return streamingSubscriptionGroupConcurrencyTest;
#else
                return streamingSubscriptionGroupConcurrency;
#endif
            }
        }
        public bool UseFileLogging
		{
			get
			{
#if DEBUG
				return useFileLoggingTest;
#else
                return useFileLogging;
#endif
			}
		}

        public bool UseConsoleLogging
        {
            get
            {
#if DEBUG
                return useConsoleLoggingTest;
#else
                return useConsoleLogging;
#endif
            }
        }

        public bool UseRemoteLogging
		{
			get
			{
#if DEBUG
				return useRemoteLoggingTest;
#else
                return useRemoteLogging;
#endif
			}
		}

		public bool UseEventLogLogging
		{
			get
			{
#if DEBUG
				return useEventLogLoggingTest;
#else
                return useEventLogLogging;
#endif
			}
		}

		public Settings()
		{
			this.ReadAll();
		}

		public void ReadAll()
		{
            try
            {
                this.dailyGetAllTimeStart = TimeSpan.Parse(this.ReadSetting("dailyGetAllStart"));
                this.dailyGetAllTimeEnd = TimeSpan.Parse(this.ReadSetting("dailyGetAllEnd"));


#if DEBUG
                if (this.ReadSetting("useConsoleLoggingTest").Equals("true")) this.useConsoleLoggingTest = true;
				if (this.ReadSetting("useFileLoggingTest").Equals("true")) this.useFileLoggingTest = true;
				if (this.ReadSetting("useRemoteLoggingTest").Equals("true")) this.useRemoteLoggingTest = true;
				if (this.ReadSetting("useEventLogLoggingTest").Equals("true")) this.useEventLogLoggingTest = true;

				this.emailListPathTest = this.ReadSetting("emailListPathTest");
				this.calendarEmailUserTest = this.ReadSetting("calendarEmailUserTest");
				this.calendarDataPathTest = this.ReadSetting("calendarDataPathTest");
				this.logPathTest = this.ReadSetting("logPathTest");

				this.exchangeUserNameTest = this.ReadSetting("exchangeUserNameTest");
				this.exchangeUserPwdTest = this.ReadSetting("exchangeUserPwdTest");
				this.calendarPostApiKeyTest = this.ReadSetting("calendarPostApiKeyTest");
				this.calendarPostApiSecretTest = this.ReadSetting("calendarPostApiSecretTest");

				if (this.ReadSetting("skipCertificateValidationTest").Equals("true")) this.skipCertificateValidationTest = true;
				if (this.ReadSetting("runForegroundTest").Equals("true")) this.runForegroundTest = true;


				if (this.ReadSetting("useHardCodedExchangeAddressTest").Equals("true"))
				{
					this.useHardCodedExchangeAddressTest = true;
					this.hardCodedExchangeAddressTest = this.ReadSetting("hardCodedExchangeAddressTest");
				}
	
                this.queueProcessorConcurrencyTest = Int32.Parse(this.ReadSetting("queueProcessorConcurrencyTest"));
                this.autodiscoverQueueProcessorConcurrencyTest = Int32.Parse(this.ReadSetting("autodiscoverQueueProcessorConcurrencyTest"));
                this.streamingSubscriptionGroupConcurrencyTest = Int32.Parse(this.ReadSetting("streamingSubscriptionGroupConcurrencyTest"));
#else
                if (this.ReadSetting("useConsoleLogging").Equals("true")) this.useConsoleLogging = true;
                if (this.ReadSetting("useFileLogging").Equals("true")) this.useFileLogging = true;
				if (this.ReadSetting("useRemoteLogging").Equals("true")) this.useRemoteLogging = true;
				if (this.ReadSetting("useEventLogLogging").Equals("true")) this.useEventLogLogging = true;

				this.emailListPath = this.ReadSetting("emailListPath");
				this.calendarEmailUser = this.ReadSetting("calendarEmailUser");
				this.calendarDataPath = this.ReadSetting("calendarDataPath");
				this.logPath = this.ReadSetting("logPath");

				this.exchangeUserName = this.ReadSetting("exchangeUserName");
				this.exchangeUserPwd = this.ReadSetting("exchangeUserPwd");
				this.calendarPostApiKey = this.ReadSetting("calendarPostApiKey");
				this.calendarPostApiSecret = this.ReadSetting("calendarPostApiSecret");

				if (this.ReadSetting("skipCertificateValidation").Equals("true")) this.skipCertificateValidation = true;
				if (this.ReadSetting("runForeground").Equals("true")) this.runForeground = true;

				if (this.ReadSetting("useHardCodedExchangeAddress").Equals("true"))
				{
					this.useHardCodedExchangeAddress = true;
					this.hardCodedExchangeAddress = this.ReadSetting("hardCodedExchangeAddress");
				}
				this.queueProcessorConcurrency = Int32.Parse(this.ReadSetting("queueProcessorConcurrency"));
                this.autodiscoverQueueProcessorConcurrency = Int32.Parse(this.ReadSetting("autodiscoverQueueProcessorConcurrency"));
                this.streamingSubscriptionGroupConcurrency = Int32.Parse(this.ReadSetting("streamingSubscriptionGroupConcurrency"));
#endif
			}
			catch (FormatException ex)
            {
                throw new Exception("Error when reading service settings", ex);
            }
            catch (OverflowException ex)
            {
                throw new Exception("Error when reading service settings", ex);
            }
        }

        private string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key];
            return result;
        }
    }
}

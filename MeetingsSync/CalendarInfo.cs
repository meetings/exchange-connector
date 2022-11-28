using System.Web.Services;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MeetingsSync
{
    class CalendarInfo
    {
        private ExchangeService service;
        private string apiKey;
        private string apiSecret;

        public static void PostCalendarInfo(CalendarItemJson json, string postAddress)
        {
            try
            {
                string postData = JsonConvert.SerializeObject(json);

                WebRequest request = WebRequest.Create(postAddress);
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
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Error sending calendar data: ", ex.Message, " FROM ", ex.InnerException ) );
            }
        }

        public CalendarInfo(Settings settings)
        {
            try
            {
                service = new ExchangeService(ExchangeVersion.Exchange2013);
                service.Credentials = new WebCredentials(settings.ExchangeUserName, settings.ExchangeUserPwd);

                if (settings.SkipCertificateValidation)
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                }

                if (settings.UseHardCodedExchangeAddress)
                {
                    service.Url = new Uri(settings.HardCodedExchangeAddress);
                }
                else
                {
                    service.AutodiscoverUrl(settings.CalendarEmailUser);
                }

                this.apiKey = settings.CalendarPostApiKey;
                this.apiSecret = settings.CalendarPostApiSecret;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Error connecting Exchange: ", ex.Message, " FROM ", ex.InnerException ) );
            }
        }

        public CalendarItemJson getCalendarInfo(string email, DateTime startDate, DateTime endDate)
        {
            Stopwatch error_timer = new Stopwatch();
            error_timer.Start();

            try
            {
                service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, email);

                CalendarItemJson appJson = new CalendarItemJson(apiKey, apiSecret);

                CalendarFolder calendar = CalendarFolder.Bind(service, new FolderId(WellKnownFolderName.Calendar, email), new PropertySet());
                CalendarView cView = new CalendarView(startDate, endDate, int.MaxValue);
                cView.PropertySet = new PropertySet(AppointmentSchema.Start, AppointmentSchema.End);
                FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);
                calendar.Load();

                appJson.email = email;
                appJson.calendar_id = calendar.Id.ToString();
                appJson.calendar_name = calendar.DisplayName;
                appJson.TimespanBeginEpoch = startDate;
                appJson.TimespanEndEpoch = endDate;

                foreach (Appointment app in appointments)
                {
                    if (app == null) continue;

                    Entries entryItem = new Entries();
                    app.Load();
                    entryItem.uid = app.Id.ToString();
                    entryItem.BeginEpoch = app.Start;
                    entryItem.EndEpoch = app.End;
                    entryItem.FreeBusyStatus = app.LegacyFreeBusyStatus.ToString();
                    entryItem.title = "busy";

                    /*
                    entryItem.title = app.Subject;
                    entryItem.description = app.Body;
                    entryItem.location = app.Location;

                    foreach (Attendee att in app.RequiredAttendees)
                    {
                        entryItem.participant_list.Add(att.Address);
                    }

                    foreach (Attendee att in app.OptionalAttendees)
                    {
                        entryItem.participant_list.Add(att.Address);
                    }
                    */

                    appJson.entries.Add(entryItem);
                }

                return appJson;
            }
            catch (Exception ex)
            {
                error_timer.Stop();
                throw new Exception(string.Concat("Error fetching data from calendar in ", error_timer.ElapsedMilliseconds, " ms: ", ex.Message, " FROM ", ex.InnerException ) );
            }
        }
    }
}

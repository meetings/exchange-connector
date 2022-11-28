using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Collections.Concurrent;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices.Autodiscover;

namespace MeetingsSync
{
    public partial class MeetingsSyncService : ServiceBase
    {
        private string lastRefreshKey = "";
        private Settings settings;

        private System.Timers.Timer timer;

        private System.Timers.Timer fullUpdateTimer;
        private DateTime lastFullUpdate;
        private bool firstFullUpdate;

        private System.Timers.Timer[] updateQueueProcessorTimers;

        // This will be actually accessed using several producer and consumer processes
        private System.Collections.Concurrent.ConcurrentQueue<UserUpdateTicket> updateQueue;

        // These are supposed to be accessed only in one thread, but might have several workers due to timeout in the future
        private System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> userUpdateTimestamps;

        private ActiveUserGrouper activeUserGrouper;
        private SubscriptionGroupManager subscriptionGroupManager;

        private static MeetingsSyncService singleton;

        //private System.Threading.Tasks.Task processWork;
        //System.Threading.CancellationTokenSource cancelToken;

        public MeetingsSyncService()
        {
            InitializeComponent();
            this.AutoLog = false;
            settings = new Settings();
            ServicePointManager.DefaultConnectionLimit = 255;
            singleton = this;
        }

        public static MeetingsSyncService getCurrent()
        {
            return singleton;
        }

        public Settings getSettings()
        {
            return this.settings;
        }

        public void debugStart(string[] args)
        {
            this.OnStart(args);
        }

        public void debugStop()
        {
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.timer = new System.Timers.Timer(100D);
                this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.DoWork);
                this.timer.AutoReset = false;
                this.timer.Start();
            }
            catch (Exception exp)
            {
                Log.Instance.WriteError("error in OnStart-method: " + exp.Message);
            }
        }

        public void DoWork(object sender, System.Timers.ElapsedEventArgs args)
        {

            Log.Instance.WriteInformation("Starting initial thread..");

            try
            {
                this.updateQueue = new System.Collections.Concurrent.ConcurrentQueue<UserUpdateTicket>();
                this.userUpdateTimestamps = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

                this.subscriptionGroupManager = new SubscriptionGroupManager(this.settings);
                this.activeUserGrouper = new ActiveUserGrouper(this.settings, this.subscriptionGroupManager);

                // Set up a timer to run heartbeat periodically.
                this.fullUpdateTimer = new System.Timers.Timer(100D); // First run after 5 second
                this.fullUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.Heartbeat);
                this.fullUpdateTimer.Start();

                // start some queue processor in separate threads
                this.updateQueueProcessorTimers = new System.Timers.Timer[settings.QueueProcessorConcurrency];
                for (int x = 0; x < this.updateQueueProcessorTimers.Length; x++)
                {
                    this.updateQueueProcessorTimers[x] = new System.Timers.Timer(5000D + x * 2000D);
                    this.updateQueueProcessorTimers[x].Elapsed += new System.Timers.ElapsedEventHandler(this.UpdateQueueProcessor);
                    this.updateQueueProcessorTimers[x].Start();
                }

                // TODO: service creation should be refactored -- this is copy paste from CalendarInfo
                Log.Instance.WriteInformation("Identifying to server as " + settings.ExchangeUserName);

                if (settings.SkipCertificateValidation)
                {
                    Log.Instance.WriteInformation("Skipping certificate validation");
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ((_sender, certificate, chain, sslPolicyErrors) => true);
                }

            }
            catch (Exception ex)
            {
                Log.Instance.WriteError(string.Concat("Error in DoWork-method: ", ex.Message, ex.InnerException));
            }
        }

        protected override void OnStop()
        {
            Log.Instance.WriteInformation("Stopping MeetingsSync service.");
            // This is a bit silly to log data that we failed to log..
            // But for some reason we sometimes failed to get other log messages and still got this, so it might work
            if (!Log.failedLogs.Equals(String.Empty))
            {
                Log.Instance.WriteInformation("Failed logs:");
                Log.Instance.WriteInformation(Log.failedLogs);
            }
            else
            {
                Log.Instance.WriteInformation("Found no failed logs.");
            }

            if (fullUpdateTimer != null && this.fullUpdateTimer.Enabled) this.fullUpdateTimer.Stop();
            for (int x = 0; x < this.updateQueueProcessorTimers.Length; x++)
            {
                if (this.updateQueueProcessorTimers[x] != null && this.updateQueueProcessorTimers[x].Enabled) this.updateQueueProcessorTimers[x].Stop();
            }
        }

        public void Heartbeat(object sender, System.Timers.ElapsedEventArgs args)
        {
            this.fullUpdateTimer.Stop();
            this.fullUpdateTimer.Interval = 3600000D; // Reserve an hour for the run at maximum
            this.fullUpdateTimer.Start();

            bool skipFull = false;

            if (this.firstFullUpdate)
            {
                // Never run full update if less than 12 hours has passed from the last update
                TimeSpan elapsed = DateTime.Now.Subtract(this.lastFullUpdate);
                if (elapsed < new TimeSpan(12, 0, 0)) skipFull = true;
            }

            int updateWindowFirstHour = settings.DailyGetAllTimeStart.Hours;
            int updateWindowLastHour = settings.DailyGetAllTimeEnd.Hours;
            int currentHour = DateTime.Now.Hour;

            if (updateWindowFirstHour > updateWindowLastHour)
            {
                if (currentHour < updateWindowFirstHour && currentHour > updateWindowLastHour) skipFull = true;
            }
            else
            {
                if (currentHour < updateWindowFirstHour || currentHour > updateWindowLastHour) skipFull = true;
            }

            if (!this.firstFullUpdate)
            {
                this.firstFullUpdate = true;
                this.lastFullUpdate = DateTime.Now;
                // don't actually run full update because on the first run updates are run at sub start
                this.doUpdate(false);
            }
            else if (skipFull)
            {
                this.doUpdate(false);
            }
            else
            {
                this.lastFullUpdate = DateTime.Now;
                this.doUpdate(true);
            }

            this.fullUpdateTimer.Stop();
            this.fullUpdateTimer.Interval = 300000D; // Make sure heartbeat is sent again after five minutes
            this.fullUpdateTimer.Start();
        }

        public void doUpdate(bool doFull)
        {
            if (doFull)
            {
                Log.Instance.WriteInformation("Refetching user list for a FULL update...");
            }
            else
            {
                Log.Instance.WriteInformation("Refetching user list for a partial update...");
            }

            try
            {
                EmailList emailList = new EmailList(settings.EmailListPath);

                string currentRefreshKey = emailList.GetRefreshKey();

                if (lastRefreshKey.Equals(""))
                {
                    lastRefreshKey = currentRefreshKey;
                }
                else if (!lastRefreshKey.Equals(currentRefreshKey))
                {
                    Log.Instance.WriteInformation("Stopping due to changed refresh key..");
                    this.Stop();
                    return;
                }

                List<string> emails = emailList.GetEmail();
                this.activeUserGrouper.SetActiveUsers(emails);

                if (doFull)
                {
                    foreach (string email in emails)
                    {
                        this.ScheduleUserUpdate(email);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteError(string.Concat(ex.Message, ex.InnerException, "While fetching user list"));
                this.Stop();
                return;
            }
        }

        public void UpdateQueueProcessor(object sender, System.Timers.ElapsedEventArgs args)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;

            // 10 minute idle failsafe for this timer to spin a new one
            timer.Stop();
            timer.Interval = 600000D;
            timer.Start();

            string latestUser = "";
            List<UserUpdateTicket> delayedTickets = new List<UserUpdateTicket>();
            UserUpdateTicket currentTicket;
            try
            {
                while (this.updateQueue.TryDequeue(out currentTicket))
                {
                    try
                    {
                        if (currentTicket.queued_time.AddSeconds(currentTicket.delay_seconds - 1) < new DateTime())
                        {
                            delayedTickets.Add(currentTicket);
                            continue;
                        }
                        // refresh idle timer for each update
                        timer.Stop();
                        timer.Interval = 600000D;
                        timer.Start();

                        // Log.Instance.WriteInformation("Dequeued user " + currentTicket.email);
                        latestUser = currentTicket.email;

                        DateTime lastStamp;
                        if (this.userUpdateTimestamps.TryGetValue(currentTicket.email, out lastStamp))
                        {
                            if (lastStamp > currentTicket.queued_time)
                            {
                                Log.Instance.WriteInformation("Skipped overlapping ticket for user " + currentTicket.email);
                                continue;
                            }
                        }
                        this.userUpdateTimestamps[currentTicket.email] = DateTime.Now;

                        Log.Instance.WriteInformation("Fetching calendar data for user " + currentTicket.email);
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        CalendarInfo calInfo = new CalendarInfo(settings);
                        CalendarItemJson itemJson = calInfo.getCalendarInfo(currentTicket.email, DateTime.Now, DateTime.Now.AddDays(90));
                        watch.Stop();
                        Log.Instance.WriteInformation("Sending calendar data for user " + currentTicket.email + " after " + watch.ElapsedMilliseconds + " ms of fetching.");
                        CalendarInfo.PostCalendarInfo(itemJson, settings.CalendarDataPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.WriteError(string.Concat("Requeuing user ", currentTicket.email, " after ", currentTicket.delay_seconds * 2 - 1, " second(s) because of update error: ", ex.Message));
                        delayedTickets.Add(new UserUpdateTicket(currentTicket.email, currentTicket.delay_seconds * 2));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteError(string.Concat("Error in dequeueing after dequeuing user ", latestUser, ": ", ex.Message));
            }

            foreach (UserUpdateTicket ticket in delayedTickets)
            {
                this.updateQueue.Enqueue(ticket);
            }

            timer.Stop();
            timer.Interval = 500D;
            timer.Start();
        }


        public void ScheduleUserUpdate(string email)
        {
            this.updateQueue.Enqueue(new UserUpdateTicket(email));
        }
        public void RegroupUserList(List<string> users)
        {
            this.activeUserGrouper.RegroupUserList(users);
        }
        public void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Instance.WriteError(string.Concat("Unhandled exception: ", e.ExceptionObject.ToString(), e.ExceptionObject.GetType() ));
            this.Stop();
        }
    }

    public class UserUpdateTicket
    {
        public string email;
        public DateTime queued_time;
        public int delay_seconds;
        public UserUpdateTicket(string user_email)
        {
            this.email = user_email;
            this.queued_time = DateTime.Now;
            this.delay_seconds = 1;
        }
        public UserUpdateTicket(string user_email, int delaySeconds)
        {
            this.email = user_email;
            this.queued_time = DateTime.Now;
            this.delay_seconds = delaySeconds;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Exchange.WebServices.Data;

namespace MeetingsSync
{
    public class SubscriptionGroup
    {
        private int maxCount = 10;

        private string subscribedUserStamp = "";
        private ConcurrentDictionary<string, bool> users = new ConcurrentDictionary<string, bool>();

        private ExchangeService service;
        private string impersonatedUser;
        private StreamingSubscriptionConnection subscriptionConnection;
        private bool subscriptionConnectionClosedIntentionally;

        private ConcurrentDictionary<string, StreamingSubscription> userSubscriptions = new ConcurrentDictionary<string, StreamingSubscription>();
        private ConcurrentDictionary<string, string> subscriptionUsers = new ConcurrentDictionary<string, string>();

        private string groupId;
        private Timer refreshTimer = new Timer(6000D);
        private bool refreshDelayed = false;

        public SubscriptionGroup(Settings settings, string groupId)
        {
            this.groupId = groupId;
            this.maxCount = settings.StreamingSubscriptionGroupConcurrency;

            this.refreshTimer.Elapsed += new ElapsedEventHandler(this.pollRefreshSubscription);
            this.refreshTimer.AutoReset = false;
            this.refreshTimer.Start();

            this.service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            this.service.Url = new Uri(settings.HardCodedExchangeAddress);
            this.service.Credentials = new WebCredentials(settings.ExchangeUserName, settings.ExchangeUserPwd);
            this.service.HttpHeaders.Add("X-PreferServerAffinity", "true");
        }

        public bool TryAddUser(string email)
        {
            if (users.Count >= this.maxCount) return false;
            users[email] = true;
            if (users.Count > this.maxCount)
            {
                bool ditch;
                users.TryRemove(email, out ditch);
                return false;
            }
            return true;
        }

        public bool TryRemoveUser(string email)
        {
            bool ditch;
            return users.TryRemove(email, out ditch);
        }

        private string createSubscribedUserStamp()
        {
            List<string> keys = users.Keys.ToList();
            return this.createSubscribedUserStamp(keys);
        }

        private string createSubscribedUserStamp(List<string> keys)
        {
            keys.Sort();
            return String.Join(" ", keys);
        }

        private void pollRefreshSubscription( object sender, ElapsedEventArgs e ) {
            AppDomain.CurrentDomain.UnhandledException += MeetingsSyncService.getCurrent().HandleUnhandledException;

            this.refreshDelayed = false;
            // 10 minute idle failsafe for this timer to spin a new one
            this.refreshTimer.Stop();
            this.refreshTimer.Interval = 600000D;
            this.refreshTimer.Start();

            string stamp = this.createSubscribedUserStamp();
            if (!stamp.Equals(this.subscribedUserStamp) || this.subscriptionConnection == null)
            {
                try {
                    lock(this)
                    {
                        this.refreshSubscription();
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.WriteError(string.Concat("Unexpected error when refreshing sub for group ",this.groupId,": ", ex.Message));
                }
            }

            if (!this.refreshDelayed)
            {
                this.refreshTimer.Stop();
                this.refreshTimer.Interval = 6000D;
                this.refreshTimer.Start();
            }
        }

        private void refreshSubscription()
        {
            if (this.subscriptionConnection != null && this.subscriptionConnection.IsOpen)
            {
                Log.Instance.WriteInformation("Stopping subscription for refresh: " + this.groupId);
                this.subscriptionConnectionClosedIntentionally = true;
                this.subscriptionConnection.Close();
                this.subscriptionConnectionClosedIntentionally = false;
            }

            List<string> keys = users.Keys.ToList();
            this.subscribedUserStamp = this.createSubscribedUserStamp(keys);

            if (keys.Count == 0)
            {
                return;
            }

            Log.Instance.WriteInformation("Starting subscription refresh: " + this.groupId);

            if (this.impersonatedUser == null || !keys.Contains(this.impersonatedUser))
            {
                this.impersonatedUser = keys[0];
            }

            this.service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, this.impersonatedUser);
            this.service.HttpHeaders.Remove("X-AnchorMailbox");
            this.service.HttpHeaders.Add("X-AnchorMailbox", this.impersonatedUser);

            if (this.subscriptionConnection == null )
            {
                this.subscriptionConnection = new StreamingSubscriptionConnection(this.service, 30);
                this.subscriptionConnection.OnNotificationEvent += this.ssOnNotificationEvent;
                this.subscriptionConnection.OnDisconnect += this.ssOnDisconnect;
                this.subscriptionConnection.OnSubscriptionError += this.ssOnError;
            }

            foreach (string email in keys)
            {
                if (this.userSubscriptions.ContainsKey(email))
                {
                    continue;
                }
                if (this.subscriptionConnection == null)
                {
                    continue;
                }

                try
                {
                    Log.Instance.WriteInformation("Adding user subscription to group " + this.groupId + ": " + email);

                    this.service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, email);

                    FolderId delegateFolder = new FolderId(WellKnownFolderName.Calendar, email);
                    this.userSubscriptions[email] = this.service.SubscribeToStreamingNotifications(
                        new FolderId[] { delegateFolder },
                        EventType.Modified,
                        EventType.FreeBusyChanged);

                    this.subscriptionUsers[this.userSubscriptions[email].Id] = email;

                    this.service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, this.impersonatedUser);

                    this.subscriptionConnection.AddSubscription(this.userSubscriptions[email]);

                    // At this point notifications from changes are already registered, and are delivered when sonnection is opened.
                    // Thus we can start updating calendars immediately.
                    MeetingsSyncService.getCurrent().ScheduleUserUpdate(email);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteInformation("Error adding user subscription to group " + this.groupId + ": " + email + " --> " + e.Message + " -- " + e.InnerException);
                    this.clearSubscriptionConnection();
                }
            }

            if (this.subscriptionConnection == null)
            {
                return;
            }

            foreach (string email in this.userSubscriptions.Keys)
            {
                if (keys.Contains(email))
                {
                    continue;
                }

                StreamingSubscription sub;
                if (this.userSubscriptions.TryRemove( email, out sub) )
                {
                    try
                    {
                        this.subscriptionConnection.RemoveSubscription(sub);
                        sub.Unsubscribe();
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.WriteInformation("Error removing user subscription from group " + this.groupId + ": " + email + " --> " + ex.Message + " -- " + ex.InnerException);
                    }
                    // NOTE: purposefully leave sub.Id to subscriptionUsers for notifications that arrive late
                }
            }

            Log.Instance.WriteInformation("Starting subscription connection after group refresh: " + this.groupId);
            try
            {
                this.subscriptionConnection.Open();
            }
            catch (Exception ex)
            {
                Log.Instance.WriteError("Error open sub connection " + this.groupId + " --> " + ex.Message + " -- " + ex.InnerException);
                this.clearSubscriptionConnection();
            }
        }

        private void ssOnNotificationEvent(object sender, NotificationEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += MeetingsSyncService.getCurrent().HandleUnhandledException;

            try
            {
                string email;
                if (this.subscriptionUsers.TryGetValue(args.Subscription.Id, out email))
                {
                    Log.Instance.WriteInformation("Processing notification event for email " + email);
                    MeetingsSyncService.getCurrent().ScheduleUserUpdate(email);
                }
                else
                {
                    Log.Instance.WriteError("Could not find email for notification from subscription " + args.Subscription.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteError(string.Concat("Error in OnNotificationEvent handler: ", ex.Message, ex.InnerException));
            }
        }

        private void ssOnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += MeetingsSyncService.getCurrent().HandleUnhandledException;

            lock(this)
            {
                if (args.Exception != null)
                {
                    Log.Instance.WriteError("StreamingSubscription disconnected with error " + args.Exception);
                    this.clearSubscriptionConnection();
                }
                else
                {
                    if (this.subscriptionConnection != null && ! this.subscriptionConnection.IsOpen && ! this.subscriptionConnectionClosedIntentionally)
                    {
                        Log.Instance.WriteInformation("Reopening normally disconnected subscription " + this.groupId);
                        this.subscriptionConnection.Open();
                    }
                }
            }
        }

        private void ssOnError(object sender, SubscriptionErrorEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += MeetingsSyncService.getCurrent().HandleUnhandledException;

            lock(this)
            {
                Exception e = args.Exception;
                Log.Instance.WriteError(string.Concat("Error in subscription connection for ", this.groupId, ": ", e.Message, e.InnerException));
                this.clearSubscriptionConnection();
            }
        }

        private void clearSubscriptionConnection()
        {
            Log.Instance.WriteInformation("Clearing subscription for " + this.groupId);

            if (this.subscriptionConnection == null)
            {
                Log.Instance.WriteInformation("Skipped clearing null subscription for " + this.groupId);

                return;
            }

            if (this.subscriptionConnection.IsOpen)
            {
                this.subscriptionConnectionClosedIntentionally = true;
                Log.Instance.WriteInformation("Closing cleared subscription for " + this.groupId);
                this.subscriptionConnection.Close();
                Log.Instance.WriteInformation("Closed cleared subscription for " + this.groupId);
                this.subscriptionConnectionClosedIntentionally = false;
            }

            foreach (string email in this.userSubscriptions.Keys)
            {

                StreamingSubscription sub;
                if (this.userSubscriptions.TryRemove(email, out sub))
                {
                    // This might be completely unnecessary but the documentation is not clear
                    // on what happens if subscriptions are not unsubscribed before Dispose
                    try
                    {
                        sub.Unsubscribe();
                    }
                    catch (Exception e)
                    {
                        e.ToString(); // Just ignore this
                    }
                }
            }

            Log.Instance.WriteInformation("Disposing cleared subscription for " + this.groupId);

            try
            {
                this.subscriptionConnection.Dispose();
            }
            catch (Exception e)
            {
                Log.Instance.WriteInformation("Error when disposing " + this.groupId + ": " + e.ToString() );
            }

            this.subscriptionConnection = null;

            this.subscribedUserStamp = "";

            // Give it a minute to do regrouping
            this.delayRefresh(60000D);

            Log.Instance.WriteInformation("Regrouping cleared subscription for " + this.groupId);
            MeetingsSyncService.getCurrent().RegroupUserList(this.users.Keys.ToList());
            Log.Instance.WriteInformation("Regrouped cleared subscription for " + this.groupId);
        }

        private void delayRefresh(double delay)
        {
            this.refreshDelayed = true;
            this.refreshTimer.Stop();
            this.refreshTimer.Interval = delay;
            this.refreshTimer.Start();
        }
    }
}

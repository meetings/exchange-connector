using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace MeetingsSync
{
    // resolves group for all added users, and adds users to their respective subcription groups
    public class ActiveUserGrouper
    {
        private ConcurrentDictionary<string, bool> activeUsers = new ConcurrentDictionary<string, bool>();
        private ConcurrentDictionary<string, string> resolvedUserGroups = new ConcurrentDictionary<string, string>();
        private ConcurrentQueue<string> resolveQueue = new ConcurrentQueue<string>();

        private System.Timers.Timer[] resolveQueueProcessorTimers;
        private Settings settings;
        private SubscriptionGroupManager groupManager;

        public ActiveUserGrouper(Settings settings, SubscriptionGroupManager groupManager )
        {
            this.settings = settings;
            this.groupManager = groupManager;

            // start some queue processor in separate threads
            this.resolveQueueProcessorTimers = new System.Timers.Timer[this.settings.AutodiscoverQueueProcessorConcurrency];
            for (int x = 0; x < this.resolveQueueProcessorTimers.Length; x++)
            {
                this.resolveQueueProcessorTimers[x] = new System.Timers.Timer(1000D + x * 500D);
                this.resolveQueueProcessorTimers[x].Elapsed += new System.Timers.ElapsedEventHandler(this.ResolveQueueProcessor);
                this.resolveQueueProcessorTimers[x].Start();
            }
        }

        public void SetActiveUsers(List<string> newUserEmails)
        {
            KeyValuePair<string,bool>[] oldUsersEmails = this.activeUsers.ToArray();

            foreach (string email in newUserEmails) {
                if (!this.activeUsers.ContainsKey(email)) {
                    this.addActiveUser(email);
                }
            }
            foreach (KeyValuePair<string, bool> email in oldUsersEmails)
            {
                if (!newUserEmails.Contains(email.Key))
                {
                    this.removeActiveUser(email.Key);
                }
            }
        }

        public void RegroupUserList(List<string> emails)
        {
            foreach (string email in emails)
            {
                if (this.activeUsers.ContainsKey(email))
                {
                    this.resolveQueue.Enqueue(email);
                }
            }
        }

        public void addActiveUser(string user)
        {
            if (this.activeUsers.TryAdd(user, true))
            {
                this.resolveQueue.Enqueue(user);
            }
        }

        public void removeActiveUser(string user)
        {
            bool discard;
            if (activeUsers.TryRemove(user, out discard))
            {
                string group;
                if (resolvedUserGroups.TryRemove(user, out group))
                {
                    this.groupManager.removeUserFromGroup(user, group);
                }
            }
        }

        private void ResolveQueueProcessor(object sender, System.Timers.ElapsedEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += MeetingsSyncService.getCurrent().HandleUnhandledException;

            System.Timers.Timer timer = (System.Timers.Timer)sender;

            // 10 minute idle failsafe for this timer to spin a new one
            timer.Stop();
            timer.Interval = 600000D;
            timer.Start();

            string currentUser = "FIRST_RUN";
            try
            {
                while (this.resolveQueue.TryDequeue(out currentUser))
                {
                    GetUserSettingsResponse UserSettings = this.GetUserSettings(currentUser);

                    bool groupExists = UserSettings.Settings.ContainsKey(UserSettingName.GroupingInformation);
                    string group = groupExists ? (string)UserSettings.Settings[UserSettingName.GroupingInformation] : "";

                    // This is some odd non-working group
                    if (groupExists && !group.Equals("Default-First-Site-Name"))
                    {
                        string oldGroup;
                        if (this.resolvedUserGroups.TryGetValue(currentUser, out oldGroup))
                        {
                            if (!oldGroup.Equals(group))
                            {
                                if (this.resolvedUserGroups.TryUpdate(currentUser, group, oldGroup))
                                {
                                    Log.Instance.WriteInformation("Reassigning user " + currentUser + " from group " + oldGroup + " to group " + group);
                                    this.groupManager.removeUserFromGroup(currentUser, oldGroup);
                                    this.groupManager.addUserToGroup(currentUser, group);
                                }
                            }
                        }
                        else
                        {
                            if (this.resolvedUserGroups.TryAdd(currentUser, group))
                            {
                                Log.Instance.WriteInformation("Assigning user " + currentUser + " to group " + group);
                                this.groupManager.addUserToGroup(currentUser, group);
                            }
                        }
                    }
                    else
                    {
                        Log.Instance.WriteError(string.Concat("No grouping information found for user ", currentUser));
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteError(string.Concat("Error autodiscovering user ", currentUser, ": ", ex.Message));
            }

            timer.Stop();
            timer.Interval = 500D;
            timer.Start();
        }

        private GetUserSettingsResponse GetUserSettings(string email)
        {
            // Attempt autodiscover, with maximum of 10 hops
            // As per MSDN: http://msdn.microsoft.com/en-us/library/office/microsoft.exchange.webservices.autodiscover.autodiscoverservice.getusersettings(v=exchg.80).aspx

            Log.Instance.WriteInformation("Autodiscovering user " + email);

            AutodiscoverService autodiscover = new AutodiscoverService(ExchangeVersion.Exchange2013_SP1);  // Minimum version we need is 2013
            autodiscover.RedirectionUrlValidationCallback = RedirectionCallback;
            autodiscover.Credentials = new WebCredentials(settings.ExchangeUserName, settings.ExchangeUserPwd);

            // Normally you would set Uri as null, but this speeds up considerably as we know they are in Office365
            Uri url = new Uri("https://autodiscover-s.outlook.com/autodiscover/autodiscover.svc");
            GetUserSettingsResponse response = null;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                autodiscover.Url = url;
                autodiscover.EnableScpLookup = false;

                response = autodiscover.GetUserSettings(email, UserSettingName.InternalEwsUrl, UserSettingName.ExternalEwsUrl, UserSettingName.GroupingInformation);

                if (response.ErrorCode == AutodiscoverErrorCode.RedirectAddress)
                {
                    return GetUserSettings(response.RedirectTarget);
                }
                else if (response.ErrorCode == AutodiscoverErrorCode.RedirectUrl)
                {
                    url = new Uri(response.RedirectTarget);
                    Log.Instance.WriteInformation("Autodiscovering user again from " + url);
                }
                else
                {
                    return response;
                }
            }

            throw new Exception("No suitable Autodiscover endpoint was found.");
        }

        static bool RedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }
    }

}

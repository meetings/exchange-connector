using System;
using System.Collections.Concurrent;

namespace MeetingsSync
{
    public class SubscriptionGroupManager
    {
        private ConcurrentDictionary<string, SubscriptionGroup> groupmap = new ConcurrentDictionary<string, SubscriptionGroup>();
        private Settings settings;

        public SubscriptionGroupManager(Settings settings)
        {
            this.settings = settings;
        }

        public void addUserToGroup(string email, string group)
        {
            for (int i = 1; i < 50; i++)
            {
                string groupId = group + '-' + i.ToString();
                SubscriptionGroup g = groupmap.GetOrAdd(groupId, (arg) => new SubscriptionGroup(settings, groupId));
                if (g.TryAddUser(email))
                {
                    return;
                }
            }
            Log.Instance.WriteError(string.Concat("Could not add user to any group: ", email, " - ", group ));
        }
        public void removeUserFromGroup(string email, string group)
        {
            for (int i = 1; i < 50; i++)
            {
                string groupId = group + '-' + i.ToString();
                SubscriptionGroup g;
                if (groupmap.TryGetValue(groupId, out g)) {
                    if (g.TryRemoveUser(email))
                    {
                        return;
                    }
                }
            }
            Log.Instance.WriteError(string.Concat("Could not remove user from any group: ", email, " - ", group ));
        }
    }
}

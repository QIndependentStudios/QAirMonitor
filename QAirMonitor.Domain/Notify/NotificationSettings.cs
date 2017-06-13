using System;

namespace QAirMonitor.Domain.Notify
{
    public class NotificationSettings
    {
        public bool IsEmailNotificationEnabled { get; set; }
        public bool IsIftttNotificationEnabled { get; set; }
        public string EmailNotificationRecipient { get; set; }
        public string IftttSecretKey { get; set; }
        public TimeSpan NotificationStartTime { get; set; }
        public TimeSpan NotificationEndTime { get; set; }
    }
}

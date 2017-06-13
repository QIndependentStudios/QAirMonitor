using System;

namespace QAirMonitor.UWP.Shared.Services
{

    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;

        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool IsEmailNotificationEnabled
        {
            get { return _helper.Read(nameof(IsEmailNotificationEnabled), false); }
            set { _helper.Write(nameof(IsEmailNotificationEnabled), value); }
        }

        public bool IsIftttNotificationEnabled
        {
            get { return _helper.Read(nameof(IsIftttNotificationEnabled), false); }
            set { _helper.Write(nameof(IsIftttNotificationEnabled), value); }
        }

        public string EmailNotificationRecipient
        {
            get { return _helper.Read(nameof(EmailNotificationRecipient), string.Empty); }
            set { _helper.Write(nameof(EmailNotificationRecipient), value); }
        }

        public string IftttSecretKey
        {
            get { return _helper.Read(nameof(IftttSecretKey), string.Empty); }
            set { _helper.Write(nameof(IftttSecretKey), value); }
        }

        public TimeSpan NotificationStartTime
        {
            get { return _helper.Read(nameof(NotificationStartTime), TimeSpan.FromHours(8)); }
            set { _helper.Write(nameof(NotificationStartTime), value); }
        }

        public TimeSpan NotificationEndTime
        {
            get { return _helper.Read(nameof(NotificationEndTime), TimeSpan.FromHours(22)); }
            set { _helper.Write(nameof(NotificationEndTime), value); }
        }
    }
}

using QAirMonitor.UWP.Shared.Services;
using System;
using Template10.Mvvm;
using Windows.ApplicationModel;

namespace QAirMonitor.UWP.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Fields
        private readonly SettingsService _settings;

        private bool _isEmailNotificationEnabled;
        private bool _isIftttNotificationEnabled;
        private string _emailRecipient;
        private string _iftttSecretKey;
        private TimeSpan _notificationStartTime = TimeSpan.FromHours(8);
        private TimeSpan _notificationEndTime = TimeSpan.FromHours(22);
        #endregion

        #region Constructors
        public SettingsPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            _settings = SettingsService.Instance;
            _isEmailNotificationEnabled = _settings.IsEmailNotificationEnabled;
            _isIftttNotificationEnabled = _settings.IsIftttNotificationEnabled;
            _emailRecipient = _settings.EmailNotificationRecipient;
            _iftttSecretKey = _settings.IftttSecretKey;
            _notificationStartTime = _settings.NotificationStartTime;
            _notificationEndTime = _settings.NotificationEndTime;
        }
        #endregion

        #region Properties
        public bool IsEmailNotificationEnabled
        {
            get { return _isEmailNotificationEnabled; }
            set { Set(ref _isEmailNotificationEnabled, value); }
        }

        public bool IsIftttNotificationEnabled
        {
            get { return _isIftttNotificationEnabled; }
            set { Set(ref _isIftttNotificationEnabled, value); }
        }

        public string EmailRecipient
        {
            get { return _emailRecipient; }
            set { Set(ref _emailRecipient, value); }
        }

        public string IftttSecretKey
        {
            get { return _iftttSecretKey; }
            set { Set(ref _iftttSecretKey, value); }
        }

        public TimeSpan NotificationStartTime
        {
            get { return _notificationStartTime; }
            set { Set(ref _notificationStartTime, value); }
        }

        public TimeSpan NotificationEndTime
        {
            get { return _notificationEndTime; }
            set { Set(ref _notificationEndTime, value); }
        }
        #endregion

        #region Event Handlers
        public void SaveSettings()
        {
            _settings.IsEmailNotificationEnabled = IsEmailNotificationEnabled;
            _settings.IsIftttNotificationEnabled = IsIftttNotificationEnabled;
            _settings.EmailNotificationRecipient = EmailRecipient;
            _settings.IftttSecretKey = IftttSecretKey;
            _settings.NotificationStartTime = NotificationStartTime;
            _settings.NotificationEndTime = NotificationEndTime;

            NavigationService.GoBack();
        }
        #endregion
    }
}

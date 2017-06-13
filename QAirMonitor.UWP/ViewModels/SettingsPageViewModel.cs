using QAirMonitor.UWP.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace QAirMonitor.UWP.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Fields
        private const int TempRangeMin = -5;
        private const int TempRangeMax = 25;
        private const int HumidityRangeMin = 0;
        private const int HumidityRangeMax = 100;

        private readonly IEnumerable<int> _temperatureValues = Enumerable.Range(TempRangeMin, TempRangeMax - TempRangeMin);
        private readonly IEnumerable<int> _humidityValues = Enumerable.Range(HumidityRangeMin, HumidityRangeMax - HumidityRangeMin);

        private readonly SettingsService _settings;

        private int _lowerTempRangeThreshold;
        private int _upperTempRangeThreshold;
        private int _lowerHumidityRangeThreshold;
        private int _upperHumidityRangeThreshold;
        private bool _isEmailNotificationEnabled;
        private bool _isIftttNotificationEnabled;
        private string _emailRecipient;
        private string _iftttSecretKey;
        private TimeSpan _notificationStartTime;
        private TimeSpan _notificationEndTime;
        #endregion

        #region Constructors
        public SettingsPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            _settings = SettingsService.Instance;
        }
        #endregion

        #region Properties
        public int LowerTempRangeThreshold
        {
            get { return _lowerTempRangeThreshold; }
            set { Set(ref _lowerTempRangeThreshold, value); }
        }

        public int UpperTempRangeThreshold
        {
            get { return _upperTempRangeThreshold; }
            set { Set(ref _upperTempRangeThreshold, value); }
        }

        public int LowerHumidityRangeThreshold
        {
            get { return _lowerHumidityRangeThreshold; }
            set { Set(ref _lowerHumidityRangeThreshold, value); }
        }

        public int UpperHumidityRangeThreshold
        {
            get { return _upperHumidityRangeThreshold; }
            set { Set(ref _upperHumidityRangeThreshold, value); }
        }

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

        public IEnumerable<int> TemperatureValues => _temperatureValues;

        public IEnumerable<int> HumidityValues => _humidityValues;
        #endregion

        #region Methods
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LowerTempRangeThreshold = _settings.LowerTempRangeThreshold;
            UpperTempRangeThreshold = _settings.UpperTempRangeThreshold;
            LowerHumidityRangeThreshold = _settings.LowerHumidityRangeThreshold;
            UpperHumidityRangeThreshold = _settings.UpperHumidityRangeThreshold;
            IsEmailNotificationEnabled = _settings.IsEmailNotificationEnabled;
            IsIftttNotificationEnabled = _settings.IsIftttNotificationEnabled;
            EmailRecipient = _settings.EmailNotificationRecipient;
            IftttSecretKey = _settings.IftttSecretKey;
            NotificationStartTime = _settings.NotificationStartTime;
            NotificationEndTime = _settings.NotificationEndTime;
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        #endregion

        #region Event Handlers
        public async void SaveSettings()
        {
            if (LowerTempRangeThreshold >= UpperTempRangeThreshold)
            {
                await new MessageDialog("Invalid temperature range.").ShowAsync();
                return;
            }


            if (LowerHumidityRangeThreshold >= UpperHumidityRangeThreshold)
            {
                await new MessageDialog("Invalid humidity range.").ShowAsync();
                return;
            }

            _settings.LowerTempRangeThreshold = LowerTempRangeThreshold;
            _settings.UpperTempRangeThreshold = UpperTempRangeThreshold;
            _settings.LowerHumidityRangeThreshold = LowerHumidityRangeThreshold;
            _settings.UpperHumidityRangeThreshold = UpperHumidityRangeThreshold;
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

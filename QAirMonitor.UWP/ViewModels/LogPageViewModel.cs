using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.System.Profile;
using Windows.UI.Xaml.Navigation;

namespace QAirMonitor.UWP.ViewModels
{
    public class LogPageViewModel : ViewModelBase
    {
        private IEnumerable<AuditLogModel> _logEntries;

        public IEnumerable<AuditLogModel> LogEntries
        {
            get { return _logEntries; }
            set { Set(ref _logEntries, value); }
        }

        public bool IsIot { get { return AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT"; } }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Refresh();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public async void Refresh()
        {
            LogEntries = await Logger.GetLogsAsync();
        }
    }
}

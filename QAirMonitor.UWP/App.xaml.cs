using Microsoft.EntityFrameworkCore;
using QAirMonitor.Business.Logging;
using QAirMonitor.Persist.Context;
using System.Threading.Tasks;
using Template10.Common;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace QAirMonitor.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : BootStrapper
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            UnhandledException += App_UnhandledException;
            InitializeComponent();
        }

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await Logger.LogExceptionAsync("App", e.Exception);
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            using (var db = new AppDataContext())
            {
                db.Database.Migrate();
            }

            NavigationService.Navigate(typeof(MainPage));
            await Task.CompletedTask;
        }
    }
}

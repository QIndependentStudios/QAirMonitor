using Microsoft.EntityFrameworkCore;
using QAirMonitor.Abstract.Business;
using QAirMonitor.Business.Logging;
using QAirMonitor.Business.SensorDataCollection;
using QAirMonitor.Domain.Models;
using QAirMonitor.Hardware.UWP.Sensors;
using QAirMonitor.Persist.Context;
using QAirMonitor.Persist.Repositories;
using QAirMonitor.UWP.Timers;
using QAirMonitor.UWP.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Windows.ApplicationModel.Activation;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.ApplicationModel;

namespace QAirMonitor.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : BootStrapper
    {
        public static readonly TimeSpan DataCollectionInterval = TimeSpan.FromMinutes(5);
        public static readonly ISensorDataCollector<ReadingModel> SensorDataCollector;
        public static readonly DateTime Startup = DateTime.Now;

        static App()
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
                SensorDataCollector = new TempHumidityDataCollector(new SnappedIntervalTimer(DataCollectionInterval),
                    new DhtTempHumiditySensor());
            else
                SensorDataCollector = new TempHumidityDataCollector(new SnappedIntervalTimer(DataCollectionInterval),
                    new VirtualTempHumiditySensor());
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            UnhandledException += App_UnhandledException;
            InitializeComponent();
        }

        private async Task StartSensorDataCollection()
        {
            var repo = new HistoricalReadingRepository();
            var Readings = await repo.GetAllAsync();
            if (Readings.Count() == 0)
            {
                var reading = new ReadingModel
                {
                    Temperature = 0,
                    Humidity = 0,
                    ReadingDateTime = DateTime.Now.Floor(DataCollectionInterval)
                };

                await repo.WriteAsync(reading);
            }

            SensorDataCollector.Start();
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
                await db.Database.MigrateAsync();
            }

            await StartSensorDataCollection();

            NavigationService.Navigate(typeof(MainPage));
            await Task.CompletedTask;
        }

        public override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            SensorDataCollector.Stop();
            return base.OnSuspendingAsync(s, e, prelaunchActivated);
        }

        public override void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            SensorDataCollector.Start();
        }
    }
}

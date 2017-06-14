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
using QAirMonitor.UWP.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace QAirMonitor.UWP
{
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

        private static void RegisterPeriodicNotifierBackgroundTask()
        {
            var hourlyTrigger = new TimeTrigger(60, false);
            //var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();
            //if (requestStatus != BackgroundAccessStatus.AlwaysAllowed)
            //{
            //    System.Diagnostics.Debug.WriteLine("Background tasks not allowed.");
            //}

            string entryPoint = typeof(Tasks.NotifyActivity).FullName;
            string taskName = "PeriodicNotifier";

            var task = RegisterBackgroundTask(entryPoint, taskName, hourlyTrigger, null);
        }

        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
            string taskName,
            IBackgroundTrigger trigger,
            IBackgroundCondition condition)
        {
            var matchingTasks = BackgroundTaskRegistration.AllTasks.Where(t => t.Value.Name == taskName);
            if (matchingTasks.Any())
                return (BackgroundTaskRegistration)(matchingTasks.FirstOrDefault().Value);

            var builder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint
            };
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(320, 480));

            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create root
                Window.Current.Content = new Shell(nav);
            }
            
            await Task.CompletedTask;
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

            RegisterPeriodicNotifierBackgroundTask();

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

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await Logger.LogExceptionAsync("App", e.Exception);
        }
    }
}

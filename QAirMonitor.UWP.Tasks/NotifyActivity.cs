using QAirMonitor.Business.Logging;
using QAirMonitor.Business.Notify;
using QAirMonitor.Domain.Notify;
using QAirMonitor.UWP.Shared.Services;
using Windows.ApplicationModel.Background;

namespace QAirMonitor.UWP.Tasks
{
    public sealed class NotifyActivity : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Query BackgroundWorkCost
            // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
            // of work in the background task and return immediately.
            //var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;


            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            _deferral = taskInstance.GetDeferral();
            await Logger.LogAsync(nameof(NotifyActivity), "Background task started.");

            var settingsService = SettingsService.Instance;
            var settings = new NotificationSettings
            {
                IsEmailNotificationEnabled = settingsService.IsEmailNotificationEnabled,
                IsIftttNotificationEnabled = settingsService.IsIftttNotificationEnabled,
                EmailNotificationRecipient = settingsService.EmailNotificationRecipient,
                IftttSecretKey = settingsService.IftttSecretKey,
                NotificationStartTime = settingsService.NotificationStartTime,
                NotificationEndTime = settingsService.NotificationEndTime
            };

            var notificationWorker = new TempHumidityNotificationWorker();
            await notificationWorker.RunAsync(settings);

            await Logger.LogAsync(nameof(NotifyActivity), "Background task completed.");
            _deferral.Complete();
        }

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await Logger.LogAsync(nameof(NotifyActivity), $"Background task canceled: {reason}");
            _deferral.Complete();
        }
    }
}

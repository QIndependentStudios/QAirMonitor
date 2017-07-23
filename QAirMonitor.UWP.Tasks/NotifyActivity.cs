using QAirMonitor.Business.Logging;
using QAirMonitor.Business.Notify;
using QAirMonitor.Domain.Notify;
using QAirMonitor.UWP.Shared.Services;
using Windows.ApplicationModel.Background;

namespace QAirMonitor.UWP.Tasks
{
    public sealed class NotifyActivity : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskCanceled;

            await Logger.LogAsync(nameof(NotifyActivity), "Background task started.");

            var settingsService = SettingsService.Instance;
            var settings = new NotificationSettings
            {
                LowerTempRangeThreshold = settingsService.LowerTempRangeThreshold,
                UpperTempRangeThreshold = settingsService.UpperTempRangeThreshold,
                LowerHumidityRangeThreshold = settingsService.LowerHumidityRangeThreshold,
                UpperHumidityRangeThreshold = settingsService.UpperHumidityRangeThreshold,
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
            _backgroundTaskDeferral.Complete();
        }

        private async void TaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await Logger.LogAsync(nameof(NotifyActivity), $"Background task canceled: {reason}");
            _backgroundTaskDeferral.Complete();
        }
    }
}

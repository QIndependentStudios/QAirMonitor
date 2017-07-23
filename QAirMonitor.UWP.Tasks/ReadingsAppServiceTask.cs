using Newtonsoft.Json;
using QAirMonitor.Business.BackgroundTask;
using QAirMonitor.Domain.BackgroundTask;
using System;
using System.Linq;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace QAirMonitor.UWP.Tasks
{
    public sealed class ReadingsAppServiceTask : IBackgroundTask
    {
        private const string CommandKey = "Command";
        private const string DataKey = "Data";
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _appServiceConnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral(); // Get a deferral so that the service isn't terminated.
            taskInstance.Canceled += TaskCanceled; // Associate a cancellation handler with the background task.

            // Retrieve the app service connection and set up a listener for incoming app service requests.
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceConnection = details.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var appServiceDeferral = args.GetDeferral();

            var message = args.Request.Message;
            var command = message[CommandKey].ToString();
            var request = JsonConvert.DeserializeObject<RemoteSessionRequest>(message[DataKey].ToString());

            var data = new ValueSet();
            var worker = new RemoteSessionAppServiceWorker();
            var items = (await worker.GetReadingsAsync(request.StartDateTime)).ToList();

            var response = new RemoteSessionResponse(items);
            data.Add(CommandKey, command);
            data.Add(DataKey, JsonConvert.SerializeObject(response));

            await args.Request.SendResponseAsync(data);
            appServiceDeferral.Complete();
        }

        private void TaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_backgroundTaskDeferral != null)
            {
                // Complete the service deferral.
                _backgroundTaskDeferral.Complete();
            }
        }
    }
}

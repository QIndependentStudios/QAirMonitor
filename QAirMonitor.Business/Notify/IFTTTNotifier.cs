using QAirMonitor.Abstract.Business;
using QAirMonitor.Domain.Notify;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace QAirMonitor.Business.Notify
{
    public class IFTTTNotifier : INotifier<Dictionary<string, string>, NotificationSettings>
    {
        public async Task SendNotificationAsync(Dictionary<string, string> data, NotificationSettings settings)
        {
            using (var client = new HttpClient())
            {
                var url = "https://maker.ifttt.com/trigger/qairmonitor_notify/with/key/" + settings.IftttSecretKey;


                var content = new FormUrlEncodedContent(data);
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
    }
}

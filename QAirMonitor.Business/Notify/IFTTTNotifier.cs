using QAirMonitor.Abstract.Business;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace QAirMonitor.Business.Notify
{
    public class IFTTTNotifier : INotifier<Dictionary<string, string>>
    {
        private const string SecretKey = "bRDqM-31cOGU7FHqtz8bFN";

        public async Task SendNotificationAsync(Dictionary<string, string> data)
        {
            using (var client = new HttpClient())
            {
                var url = "https://maker.ifttt.com/trigger/qairmonitor_notify/with/key/" + SecretKey;


                var content = new FormUrlEncodedContent(data);
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
    }
}

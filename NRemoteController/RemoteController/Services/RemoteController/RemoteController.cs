using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace RemoteController.Services.RemoteController
{
    public class RemoteController
    {
        private readonly string _ipAddress;

        public RemoteController(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public async Task<bool> SendRemoteCommandAsync(string pressedKey)
        {
            bool succeeded = false;
            string address = String.Empty;

            if (!string.IsNullOrEmpty(pressedKey) && !string.IsNullOrEmpty(_ipAddress))
            {
                address = "http://" + _ipAddress + "/RemoteControl/KeyHandling/sendKey?key=" + pressedKey;

                var uri = new Uri(address, UriKind.Absolute);

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request.RequestUri = new Uri(address);

                    IHttpContent httpContent = new HttpStringContent(String.Empty);
                    try
                    {
                        await client.PostAsync(uri, httpContent);
                    }
                    catch (Exception)
                    {
                        return succeeded = false;
                    }

                    return succeeded = true;
                }
            }
            return succeeded = false;
        }
    }
}

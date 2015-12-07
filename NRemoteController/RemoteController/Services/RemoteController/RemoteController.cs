using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;
//using System.Net.Http;
//using System.Net.Http.Headers;
using Newtonsoft.Json;
using RemoteController.Models;

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

            if (!string.IsNullOrWhiteSpace(pressedKey) && !string.IsNullOrWhiteSpace(_ipAddress))
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

        public async Task<List<TvChannels>> GetChannelListAsync()
        {
            List<TvChannels> tvChannelList = new List<TvChannels>();
            string address = String.Empty;

            if (!string.IsNullOrWhiteSpace(_ipAddress))
            {
                address = "http://" + _ipAddress + "/Live/Channels/getList";

                var uri = new Uri(address, UriKind.Absolute);

                //using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                //{
                //    try
                //    {

                //        var headers = httpClient.DefaultRequestHeaders;

                //        headers.UserAgent.ParseAdd(
                //            "Netia%C2%A0Player%C2%A0Pilot/2014.10.271657 CFNetwork/758.1.6 Darwin/15.0.0");


                //        var content = await httpClient.GetStreamAsync(uri);

                //        //var content = await httpClient.GetStringAsync(uri);

                //        var intt = content.Length;

                //        tvChannelList = JsonConvert.DeserializeObject<List<TvChannels>>(content.ToString());

                //        return tvChannelList;

                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //}

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");

                    System.Net.Http.HttpResponseMessage response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsStringAsync();
                    tvChannelList = JsonConvert.DeserializeObject<List<TvChannels>>(data);

                    return tvChannelList;
                }

            }
            return tvChannelList;
        }
    }
}

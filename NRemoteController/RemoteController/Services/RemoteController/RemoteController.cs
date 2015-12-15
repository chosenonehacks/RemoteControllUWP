using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
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

        public async Task<bool> SendRemoteCommandForAppAsync(string pressedApp)
        {
            bool succeeded = false;
            string address = String.Empty;

            if (!string.IsNullOrWhiteSpace(pressedApp) && !string.IsNullOrWhiteSpace(_ipAddress))
            {
                
                address = "http://" + _ipAddress + "/Applications/Lifecycle/open?appId=" + pressedApp;

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


#region different way to send httpClient
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
#endregion

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
                    try
                    {
                        System.Net.Http.HttpResponseMessage response = await client.GetAsync(uri);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsStringAsync();
                        tvChannelList = JsonConvert.DeserializeObject<List<TvChannels>>(data);

                        return tvChannelList;
                    }
                    catch (Exception)
                    {

                        return tvChannelList;
                    }
                    
                }

            }
            return tvChannelList;
        }

        public async Task<bool> SendRemoteCommandByZapAsync(string pressedZap)
        {
            if (!String.IsNullOrEmpty(pressedZap))
            {
                var keysArray = pressedZap.ToCharArray();
                bool sendRemoteResult;
                List<bool> sendRemoteResultsList = new List<bool>();

                foreach (char k in keysArray)
                {
                    sendRemoteResult = await SendRemoteCommandAsync(k.ToString());
                    sendRemoteResultsList.Add(sendRemoteResult);
                }
                return sendRemoteResultsList.All(x => x != false); //if any from the list is not false return true
            }
            
            return false;
        }

        public async Task<bool> SendRemoteCommandByAppIdAsync(string pressedApp)
        {
            if (!String.IsNullOrEmpty(pressedApp))
            {
                
                bool sendRemoteResult = false;
                
                    sendRemoteResult = await SendRemoteCommandForAppAsync(pressedApp);

                return sendRemoteResult;
            }

            return false;
        }

        public async Task<List<NetiaApp>> GetAppsListAsync()
        {
            RootObject rootObject = new RootObject();
            List<NetiaApp> appList = new List<NetiaApp>();

            //http://epg.dms.netia.pl/xmltv/logo/black/netiaPadAppsSettings.json
            var address = "http://epg.dms.netia.pl/xmltv/logo/black/netiaPadAppsSettings.json";

                var uri = new Uri(address, UriKind.Absolute);
                

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
                    try
                    {
                        System.Net.Http.HttpResponseMessage response = await client.GetAsync(uri);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsStringAsync();

                        rootObject = JsonConvert.DeserializeObject<RootObject>(data);

                        appList = enumrateRootObjectApps(rootObject);

                        return appList;
                    }
                    catch (Exception)
                    {
                        return appList; ; //empty
                    }

                }
        }

        #region Enumerate RootObject
        private List<NetiaApp> enumrateRootObjectApps(RootObject rootObject)
        {
            var propArray = rootObject.apps.GetType().GetProperties();
            List<NetiaApp> appList = new List<NetiaApp>();

            foreach (var propertyInfo in propArray)
            {
                if (propertyInfo.Name == rootObject.apps.abcvod.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.abcvod.Id,
                        Name = rootObject.apps.abcvod.Name,
                        Description = rootObject.apps.abcvod.Description,
                        GridImg_url = rootObject.apps.abcvod.GridImg_url,
                        ListImg_url = rootObject.apps.abcvod.ListImg_url,
                        isActive = rootObject.apps.abcvod.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.goon.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.goon.Id,
                        Name = rootObject.apps.goon.Name,
                        Description = rootObject.apps.goon.Description,
                        GridImg_url = rootObject.apps.goon.GridImg_url,
                        ListImg_url = rootObject.apps.goon.ListImg_url,
                        isActive = rootObject.apps.goon.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.erowizja.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.erowizja.Id,
                        Name = rootObject.apps.erowizja.Name,
                        Description = rootObject.apps.erowizja.Description,
                        GridImg_url = rootObject.apps.erowizja.GridImg_url,
                        ListImg_url = rootObject.apps.erowizja.ListImg_url,
                        isActive = rootObject.apps.erowizja.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.filmbox.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.filmbox.Id,
                        Name = rootObject.apps.filmbox.Name,
                        Description = rootObject.apps.filmbox.Description,
                        GridImg_url = rootObject.apps.filmbox.GridImg_url,
                        ListImg_url = rootObject.apps.filmbox.ListImg_url,
                        isActive = rootObject.apps.filmbox.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.hbogo.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.hbogo.Id,
                        Name = rootObject.apps.hbogo.Name,
                        Description = rootObject.apps.hbogo.Description,
                        GridImg_url = rootObject.apps.hbogo.GridImg_url,
                        ListImg_url = rootObject.apps.hbogo.ListImg_url,
                        isActive = rootObject.apps.hbogo.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.ipla.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.ipla.Id,
                        Name = rootObject.apps.ipla.Name,
                        Description = rootObject.apps.ipla.Description,
                        GridImg_url = rootObject.apps.ipla.GridImg_url,
                        ListImg_url = rootObject.apps.ipla.ListImg_url,
                        isActive = rootObject.apps.ipla.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.kinoplex.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.kinoplex.Id,
                        Name = rootObject.apps.kinoplex.Name,
                        Description = rootObject.apps.kinoplex.Description,
                        GridImg_url = rootObject.apps.kinoplex.GridImg_url,
                        ListImg_url = rootObject.apps.kinoplex.ListImg_url,
                        isActive = rootObject.apps.kinoplex.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.tvpsport.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.tvpsport.Id,
                        Name = rootObject.apps.tvpsport.Name,
                        Description = rootObject.apps.tvpsport.Description,
                        GridImg_url = rootObject.apps.tvpsport.GridImg_url,
                        ListImg_url = rootObject.apps.tvpsport.ListImg_url,
                        isActive = rootObject.apps.tvpsport.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.ninateka.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.ninateka.Id,
                        Name = rootObject.apps.ninateka.Name,
                        Description = rootObject.apps.ninateka.Description,
                        GridImg_url = rootObject.apps.ninateka.GridImg_url,
                        ListImg_url = rootObject.apps.ninateka.ListImg_url,
                        isActive = rootObject.apps.ninateka.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.netiacloud.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.netiacloud.Id,
                        Name = rootObject.apps.netiacloud.Name,
                        Description = rootObject.apps.netiacloud.Description,
                        GridImg_url = rootObject.apps.netiacloud.GridImg_url,
                        ListImg_url = rootObject.apps.netiacloud.ListImg_url,
                        isActive = rootObject.apps.netiacloud.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.tubafm.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.tubafm.Id,
                        Name = rootObject.apps.tubafm.Name,
                        Description = rootObject.apps.tubafm.Description,
                        GridImg_url = rootObject.apps.tubafm.GridImg_url,
                        ListImg_url = rootObject.apps.tubafm.ListImg_url,
                        isActive = rootObject.apps.tubafm.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.tvnmeteo.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.tvnmeteo.Id,
                        Name = rootObject.apps.tvnmeteo.Name,
                        Description = rootObject.apps.tvnmeteo.Description,
                        GridImg_url = rootObject.apps.tvnmeteo.GridImg_url,
                        ListImg_url = rootObject.apps.tvnmeteo.ListImg_url,
                        isActive = rootObject.apps.tvnmeteo.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.tvnplayer.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.tvnplayer.Id,
                        Name = rootObject.apps.tvnplayer.Name,
                        Description = rootObject.apps.tvnplayer.Description,
                        GridImg_url = rootObject.apps.tvnplayer.GridImg_url,
                        ListImg_url = rootObject.apps.tvnplayer.ListImg_url,
                        isActive = rootObject.apps.tvnplayer.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.pinkvision.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.pinkvision.Id,
                        Name = rootObject.apps.pinkvision.Name,
                        Description = rootObject.apps.pinkvision.Description,
                        GridImg_url = rootObject.apps.pinkvision.GridImg_url,
                        ListImg_url = rootObject.apps.pinkvision.ListImg_url,
                        isActive = rootObject.apps.pinkvision.isActive
                    });
                }

                if (propertyInfo.Name == rootObject.apps.premiumplus.Id)
                {
                    appList.Add(new NetiaApp()
                    {
                        Id = rootObject.apps.premiumplus.Id,
                        Name = rootObject.apps.premiumplus.Name,
                        Description = rootObject.apps.premiumplus.Description,
                        GridImg_url = rootObject.apps.premiumplus.GridImg_url,
                        ListImg_url = rootObject.apps.premiumplus.ListImg_url,
                        isActive = rootObject.apps.premiumplus.isActive
                    });
                }

            }

            return appList;
        }
#endregion
    }
}



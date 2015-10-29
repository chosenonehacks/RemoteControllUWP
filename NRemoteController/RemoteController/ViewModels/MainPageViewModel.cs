using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class MainPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime data
                Value = "Designtime value";
                return;
            }
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            {
                // use cache value(s)
                if (state.ContainsKey(nameof(Value))) Value = state[nameof(Value)]?.ToString();
                // clear any cache
                state.Clear();
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                // persist into cache
                state[nameof(Value)] = Value;
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(NavigatingEventArgs args)
        {
            base.OnNavigatingFrom(args);
        }

        private string _Value = string.Empty;
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public void GotoDetailsPage()
        {
            this.NavigationService.Navigate(typeof(Views.DetailPage), this.Value);
        }

        private DelegateCommand<string> _setPilotCommand;

        public DelegateCommand<string> SendPilotCommand
        {
            get
            {
                if (_setPilotCommand == null)
                {
                    _setPilotCommand = new DelegateCommand<string>((s) =>
                    {
                        SendRemoteCommand(s);
                    }/*, (s) => !string.IsNullOrEmpty(Value)*/); // can do check

                }
                return _setPilotCommand;
            }
        }

        private async void SendRemoteCommand(string s)
        {
            string address;
            if (!string.IsNullOrEmpty(s))
            {
                address = "http://" + "192.168.1.4/RemoteControl/KeyHandling/sendKey?key=" + s;

                var uri = new Uri(address,UriKind.Absolute);
                
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();
                    
                    request.RequestUri = new Uri(address);
                    
                    IHttpContent httpContent = new HttpStringContent(String.Empty);
                    
                    await client.PostAsync(uri, httpContent);
                    //HttpResponseMessage response = await client.PostAsync(uri, httpContent);
                    //var con = response.Content;
                }
            }
        }

        #region Command Example
        //private DelegateCommand _setPilotCommand;

        //public DelegateCommand SendPilotCommand
        //{
        //    get
        //    {
        //        if (_setPilotCommand == null)
        //        {
        //            _setPilotCommand = new DelegateCommand(() =>
        //            {
        //                Result = $"Hello {Name}";
        //            }, () => !string.IsNullOrEmpty(Name));

        //        }

        //        return _setPilotCommand;

        //    }
        //}
        #endregion
    }
}


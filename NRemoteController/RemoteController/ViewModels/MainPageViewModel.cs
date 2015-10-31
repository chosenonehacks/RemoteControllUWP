using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using RemoteController.Services.DialogServices;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class MainPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private Services.SettingsServices.SettingsService _settings;
        private Services.DialogServices.DialogService _dialog;
        
        public MainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //No IpAdress saved go to Settings Page
            await CheckIpAddressSetting();
        }

        private async Task CheckIpAddressSetting()
        {
            if (IpAddress == String.Empty)
            {
                _dialog = new DialogService();
                UICommand okBtn = new UICommand("OK");
                await _dialog.ShowAsync("Looks like you didn't setup IP address of your Netgem box. We will go to Settings page.", "Setup", okBtn);
                GotoSettingsPage();
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            
            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(NavigatingEventArgs args)
        {
            base.OnNavigatingFrom(args);
        }

        private string _IpAddress;
        public string IpAddress
        {
            get { return _settings.IpAddress; }
            set { _settings.IpAddress = _IpAddress; base.RaisePropertyChanged(); }
        }

        public void GotoSettingsPage()
        {
            this.NavigationService.Navigate(typeof(Views.SettingsPage));
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
                    }/*, (pressedKey) => !string.IsNullOrEmpty(Value)*/); // can do check

                }
                return _setPilotCommand;
            }
        }

        private async void SendRemoteCommand(string pressedKey)
        {
            string address = String.Empty;

            if (!string.IsNullOrEmpty(pressedKey) && !string.IsNullOrEmpty(IpAddress))
            {
                address = "http://" + IpAddress + "/RemoteControl/KeyHandling/sendKey?key=" + pressedKey;

                var uri = new Uri(address,UriKind.Absolute); //Exception incase of invalid IP address entere some validation required
                
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
    }
}


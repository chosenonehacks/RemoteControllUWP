using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace RemoteController.ViewModels
{
    public class MainPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;
        private readonly ISettingsManager _manager;
        private DialogService _dialog;
        public MainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
                _manager = new LocalSettingsManager();
            }
        }

        public async override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //No IpAdress saved go to settingsPage
            if (String.IsNullOrWhiteSpace(IpAddress))
            {
                _dialog = new DialogService();
                UICommand okBtn = new UICommand("OK");
                await _dialog.ShowAsync("No IP settings is saved", "IP Setup",okBtn);
                GotoSettingsPage();
            }
        }

        public string IpAddress
        {
            get { return _manager.Load<string>("IpSetting", String.Empty); }
        }

        public void GotoSettingsPage()
        {
            this.NavigationService.Navigate(typeof(Views.SettingsPage));
        }

        public void GotoKeyboardPage()
        {
            this.NavigationService.Navigate(typeof(Views.KeyboardPage));
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

        private DelegateCommand _gotoKeyboardPageComaCommand;

        public DelegateCommand GotoKeyboardPageCommand
        {
            get
            {
                if (_gotoKeyboardPageComaCommand == null)
                {
                    _gotoKeyboardPageComaCommand = new DelegateCommand(GotoKeyboardPage); 
                }
                return _gotoKeyboardPageComaCommand;
            }
        }

        private async void SendRemoteCommand(string pressedKey)
        {
            string address = String.Empty;

            if (!string.IsNullOrEmpty(pressedKey) && !string.IsNullOrEmpty(IpAddress))
            {
                address = "http://" + IpAddress + "/RemoteControl/KeyHandling/sendKey?key=" + pressedKey;

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
    }
}


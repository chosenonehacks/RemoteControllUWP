using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using RemoteController.Models;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class AppListViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;
        private readonly Services.RemoteController.RemoteController _remoteController;
        private DialogService _dialog;
        private ResourceLoader _loader;

        public AppListViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
                _remoteController = new Services.RemoteController.RemoteController(IpAddress);
                _loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            }
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await GetAppListAsync();
        }

        private List<NetiaApp> _listOfApps;
        public List<NetiaApp> ListOfApps
        {
            get { return _listOfApps; }
            set { Set(ref _listOfApps, value); }
        }

        //TODO:maybe in future add rest of functions that are on android and IOS version
        public async Task GetAppListAsync()
        {
            ShowBusy();

            ListOfApps = new List<NetiaApp>();
            ListOfApps = await _remoteController.GetAppsListAsync();
            

            if (!ListOfApps.Any())
            {
                await ShowDialogAsync("NoConnection");
                HideBusy();
                GotoSettingsPage();
            }
            else
            {
                HideBusy();
            }
            

        }

        public void GotoSettingsPage()
        {
            this.NavigationService.Navigate(typeof(Views.SettingsPage));
        }

        public void GotoMainPage()
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));
        }

        private string _BusyText;
        public string BusyText
        {
            get { return _BusyText; }
            set { Set(ref _BusyText, value); }
        }

        public void ShowBusy()
        {
            Views.Shell.SetBusyVisibility(Visibility.Visible, _BusyText);
        }

        public void HideBusy()
        {
            Views.Shell.SetBusyVisibility(Visibility.Collapsed);
        }

        private DelegateCommand<string> _setPilotCommand;
        public DelegateCommand<string> SendPilotCommand
        {
            get
            {
                if (_setPilotCommand == null)
                {
                    _setPilotCommand = new DelegateCommand<string>(async (s) =>
                    {
                        await SendRemoteCommandAsync(s);
                    });
                }
                return _setPilotCommand;
            }
        }

        private async Task SendRemoteCommandAsync(string pressedApp)
        {
            var result = await _remoteController.SendRemoteCommandByAppIdAsync(pressedApp); 

            if (!result)
            {
                await ShowDialogAsync("NoConnection");
            }
            else
            {
                GotoMainPage();
            }
        }

        private async Task ShowDialogAsync(string message)
        {
            _dialog = new DialogService();

            var messageHeader = _loader.GetString(string.Format("{0}Header", message));
            var messageContent = _loader.GetString(string.Format("{0}Content", message));

            await _dialog.ShowAsync(messageContent, messageHeader, new UICommand("OK"));
        }  

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);
    }
}

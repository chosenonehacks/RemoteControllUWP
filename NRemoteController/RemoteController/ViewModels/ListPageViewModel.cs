using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using RemoteController.Models;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class ListPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;
        private readonly Services.RemoteController.RemoteController _remoteController;
        private DialogService _dialog;
        private ResourceLoader _loader;

        public ListPageViewModel()
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
            await GetChannelsListAsync();
        }

        private List<TvChannels> _listOfTvChannels;
        public List<TvChannels> ListOfTvChannels
        {
            get { return _listOfTvChannels; }
            set { Set(ref _listOfTvChannels, value); }
        }

        //TODO:maybe in future add rest of functions that are on android and IOS version
        public async Task GetChannelsListAsync()
        {
            ListOfTvChannels = new List<TvChannels>();
            ListOfTvChannels = await _remoteController.GetChannelListAsync();
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
                    }/*, (pressedKey) => !string.IsNullOrEmpty(Value)*/); // can do check
                }
                return _setPilotCommand;
            }
        }

        private async Task SendRemoteCommandAsync(string pressedZap)
        {
            var result = await _remoteController.SendRemoteCommandByZapAsync(pressedZap); 

            if (!result)
            {
                await ShowDialogAsync("NoConnection");
                //await _dialog.ShowAsync("Połączenie do dekodera Netia nie może być ustanowione. Proszę sprawdzić ustawienia adresu IP.", "Problem z siecią", new UICommand("OK"));
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

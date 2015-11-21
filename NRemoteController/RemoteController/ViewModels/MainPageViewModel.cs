using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class MainPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;
        private DialogService _dialog;
        private readonly Services.RemoteController.RemoteController _remoteController;
        private ResourceLoader _loader;

        public MainPageViewModel()
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
            //No IpAdress saved go to settingsPage
            if (String.IsNullOrWhiteSpace(IpAddress))
            {
                await ShowDialogAsync("NoIpSetting");
                GotoSettingsPage();
            }
        }

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);

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
                    _setPilotCommand = new DelegateCommand<string>(async (s) =>
                    {
                       await SendRemoteCommandAsync(s);
                    }/*, (pressedKey) => !string.IsNullOrEmpty(Value)*/); // can do check
                }
                return _setPilotCommand;
            }
        }

        private DelegateCommand _gotoKeyboardPageComaCommand;

        public DelegateCommand GotoKeyboardPageCommand => _gotoKeyboardPageComaCommand ??
                                                          (_gotoKeyboardPageComaCommand = new DelegateCommand(GotoKeyboardPage));

        private async Task SendRemoteCommandAsync(string pressedKey)
        {
            var result = await _remoteController.SendRemoteCommandAsync(pressedKey);

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
    }
}


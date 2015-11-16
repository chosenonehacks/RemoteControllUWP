using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
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

        public MainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
                _remoteController = new Services.RemoteController.RemoteController(IpAddress);
                _dialog = new DialogService();
            }
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //No IpAdress saved go to settingsPage
            if (String.IsNullOrWhiteSpace(IpAddress))
            {
                UICommand okBtn = new UICommand("OK");
                await _dialog.ShowAsync("No IP settings is saved.", "IP Setup",okBtn);
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
                await _dialog.ShowAsync("Connection to your box cannot be established. Please check your IP Address settings.", "Network Problem", new UICommand("OK"));
            }
        }
    }
}


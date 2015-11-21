using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Popups;
using Windows.Web.Http;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class KeyboardPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;
        private DialogService _dialog;
        private readonly Services.RemoteController.RemoteController _remoteController;
        private ResourceLoader _loader;

        public KeyboardPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
                _remoteController = new Services.RemoteController.RemoteController(IpAddress);
                _loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            }
        }

        public async void KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
                return;

            var key = e.Key;
            
            //TODO: It always send big letters, check possibility to make a difference small and big ones.

            //TODO: Clear textbox upon enter, check keyboardService for that prupose
            switch (key)
            {
                case VirtualKey.Number0:
                    await SendRemoteCommand("0");
                    break;
                case VirtualKey.Number1:
                    await SendRemoteCommand("1");
                    break;
                case VirtualKey.Number2:
                    await SendRemoteCommand("2");
                    break;
                case VirtualKey.Number3:
                    await SendRemoteCommand("3");
                    break;
                case VirtualKey.Number4:
                    await SendRemoteCommand("4");
                    break;
                case VirtualKey.Number5:
                    await SendRemoteCommand("5");
                    break;
                case VirtualKey.Number6:
                    await SendRemoteCommand("6");
                    break;
                case VirtualKey.Number7:
                    await SendRemoteCommand("7");
                    break;
                case VirtualKey.Number8:
                    await SendRemoteCommand("8");
                    break;
                case VirtualKey.Number9:
                    await SendRemoteCommand("9");
                    break;
                case VirtualKey.Back:
                    await SendRemoteCommand("delete");
                    break;
                case VirtualKey.Space:
                    await SendRemoteCommand("KEY_SPACE");
                    break;
                case VirtualKey.Enter:
                    await SendRemoteCommand("ok");
                    break;
                default:
                    await SendRemoteCommand(key.ToString());
                    break;
            }
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
                        await SendRemoteCommand(s);
                    });
                }
                return _setPilotCommand;
            }
        }

        private async Task SendRemoteCommand(string pressedKey)
        {
            var result = await _remoteController.SendRemoteCommandAsync(pressedKey);

            if (!result)
            {
                await ShowDialogAsync("NoConnection");
            }
        }

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);

        private async Task ShowDialogAsync(string message)
        {
            _dialog = new DialogService();

            var messageHeader = _loader.GetString(string.Format("{0}Header", message));
            var messageContent = _loader.GetString(string.Format("{0}Content", message));

            await _dialog.ShowAsync(messageContent, messageHeader, new UICommand("OK"));
        }
    }
}

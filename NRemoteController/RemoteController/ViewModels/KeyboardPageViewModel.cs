using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.Web.Http;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class KeyboardPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;

        public KeyboardPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
            }
        }

        public void KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
                return;

            var key = e.Key;
            
            //TODO: It always send big letters, check possibility to make a difference small and big ones.

            //TODO: Clear textbox upon enter, check keyboardService for that prupose
            switch (key)
            {
                case VirtualKey.Number0:
                    SendRemoteCommand("0");
                    break;
                case VirtualKey.Number1:
                    SendRemoteCommand("1");
                    break;
                case VirtualKey.Number2:
                    SendRemoteCommand("2");
                    break;
                case VirtualKey.Number3:
                    SendRemoteCommand("3");
                    break;
                case VirtualKey.Number4:
                    SendRemoteCommand("4");
                    break;
                case VirtualKey.Number5:
                    SendRemoteCommand("5");
                    break;
                case VirtualKey.Number6:
                    SendRemoteCommand("6");
                    break;
                case VirtualKey.Number7:
                    SendRemoteCommand("7");
                    break;
                case VirtualKey.Number8:
                    SendRemoteCommand("8");
                    break;
                case VirtualKey.Number9:
                    SendRemoteCommand("9");
                    break;
                case VirtualKey.Back:
                    SendRemoteCommand("delete");
                    break;
                case VirtualKey.Space:
                    SendRemoteCommand("KEY_SPACE");
                    break;
                case VirtualKey.Enter:
                    SendRemoteCommand("ok");
                    break;
                default:
                    SendRemoteCommand(key.ToString());
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

                var uri = new Uri(address, UriKind.Absolute);

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request.RequestUri = new Uri(address);

                    IHttpContent httpContent = new HttpStringContent(String.Empty);

                    await client.PostAsync(uri, httpContent);
                }
            }
        }

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);
    }
}

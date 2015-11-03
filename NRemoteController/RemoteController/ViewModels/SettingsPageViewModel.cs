using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace RemoteController.ViewModels
{
    public class SettingsPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();

        public async override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {

            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(NavigatingEventArgs args)
        {
            base.OnNavigatingFrom(args);
        }

    }

    public class SettingsPartViewModel : Mvvm.ViewModelBase
    {
        private Services.SettingsServices.SettingsService _settings;
        private ISettingsManager _manager;
        private DialogService _dialog;

        public SettingsPartViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
                _manager = new LocalSettingsManager();
            }
            
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set
            {
                _settings.UseShellBackButton = value;
                base.RaisePropertyChanged();
            }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set
            {
                _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark;
                base.RaisePropertyChanged();
            }
        }

        public string IpAddress
        {
            get { return _manager.Load<string>("IpSetting",String.Empty); }
            set
            {
                //_settings.IpAddress = value;
                _manager.Save("IpSetting", value);
                base.RaisePropertyChanged();
            }
        }

        private string _selectedIpAddress;
        public string SelectedIpAddress
        {
            get { return _selectedIpAddress; }
            set { Set(ref _selectedIpAddress, value); }
        }

        private ObservableCollection<string> _listOfScannedIpAddresse;
        public ObservableCollection<string> ListOfScannedIpAddresses
        {
            get { return _listOfScannedIpAddresse; }
            set { Set(ref _listOfScannedIpAddresse, value); }
        }

        private bool _isIpListVisible;
        public bool IsIpListVisible
        {
            get { return _isIpListVisible; }
            set { Set(ref _isIpListVisible, value); }
        }
        private bool _isSearchingVisible;
        public bool IsSearchingVisible
        {
            get { return _isSearchingVisible; }
            set { Set(ref _isSearchingVisible, value); }
        }



        private DelegateCommand _scanNetworkCommand;
        public DelegateCommand ScanNetworkCommand
        {
            get
            {
                if (_scanNetworkCommand == null)
                {
                    _scanNetworkCommand = new DelegateCommand(ScanLocalNetwork);
                }
                return _scanNetworkCommand;
            }
        }

        private DelegateCommand<string> _selectIpAddressFromListCommand;

        public DelegateCommand<string> SelectIpAddressFromListCommand
        {
            get
            {
                if (_selectIpAddressFromListCommand == null)
                {
                    _selectIpAddressFromListCommand = new DelegateCommand<string>((s) =>
                    {
                        //SelectIpAddressFromList(s);
                    });

                }
                return _selectIpAddressFromListCommand;
            }
        }

        public void SelectIpAddressFromList(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                SelectedIpAddress = listView.SelectedItem.ToString();
                IpAddress = SelectedIpAddress;
                IsIpListVisible = false;
            }
        }

        private DelegateCommand<string> _checkIpAddressCommand;

        public DelegateCommand<string> CheckIpAddressCommand
        {
            get
            {
                if (_checkIpAddressCommand == null)
                {
                    _checkIpAddressCommand = new DelegateCommand<string>((s) =>
                    {
                        CheckIpAddress(s);
                    });

                }
                return _checkIpAddressCommand;
            }
        }

        private string _BusyText = "Please wait...";
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

        private async void CheckIpAddress(string ipAddressToCheck)
        {
            //TODO:validate IP
            if (IsValidIp(ipAddressToCheck))
            {
                //TODO:send http request with answer
                
                    ShowBusy();
                    bool validAddress = await SendHttpRequest(ipAddressToCheck);

                if (validAddress)
                {
                    HideBusy();
                    _dialog = new DialogService();
                    await _dialog.ShowAsync("You can use this IP to control your Netgem or Netia box", "Good IP Address", new UICommand("OK"));
                    //GoToPilotPage();
                }
                else
                {
                    HideBusy();
                    _dialog = new DialogService();
                    await _dialog.ShowAsync("Change and recheck IP address.", "Wrong IP Address", new UICommand("OK"));
                }
            }
        }

        private void GoToPilotPage()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();   
            }
        }

        private async Task<bool> SendHttpRequest(string ipAddressToCheck)
        {
            HttpResponseMessage reposneMsg;
            string address = String.Empty;

            //TEMP
            //ipAddressToCheck = "192.168.1.4";

            address = "http://" + ipAddressToCheck + "/RemoteControl/Volume/get";
            var uri = new Uri(address, UriKind.Absolute);

            using (HttpClient client = new HttpClient())
            {
                //IsChecking = true;
                try
                {
                    reposneMsg = await client.GetAsync(uri);
                    return reposneMsg.IsSuccessStatusCode;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        //private bool IsChecking { get; set; }

        private bool IsValidIp(string ipAddressToCheck)
        {
            if(ipAddressToCheck == null) throw new ArgumentException("ipAddressToCheck");

            IPAddress ipAddres;
            return IPAddress.TryParse(ipAddressToCheck, out ipAddres);
        }

        private async void ScanLocalNetwork()
        {
            string localIpAddress = GetLocalIpAddress();

            if (!string.IsNullOrEmpty(localIpAddress))
            {
                List<IPAddress> IpAddressList = new List<IPAddress>();
                IpAddressList = GetListOfLocalIpAddresses(localIpAddress);
                ListOfScannedIpAddresses = new ObservableCollection<string>();

                foreach (var ipAddress in IpAddressList)
                {
                    //ShowBusy();
                    IsSearchingVisible = true; //show progress ring for list

                    var validAddress = await SendHttpRequest(ipAddress.ToString());

                    if (validAddress)
                    {
                        ListOfScannedIpAddresses.Add(ipAddress.ToString());
                        break;
                    }
                }

                //HideBusy();
                IsIpListVisible = true;
                IsSearchingVisible = false;
            }

        }

        public string GetLocalIpAddress()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                List<string> ipAddresses = new List<string>();

                var hostnames = NetworkInformation.GetHostNames();
                foreach (var hn in hostnames)
                {
                    if (hn.IPInformation != null)
                    {
                        if (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6 || // An Ethernet network interface.
                            hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71)
                            // An IEEE 802.11 wireless network interface.
                        {
                            IPAddress validIpAddress;

                            var valid = IPAddress.TryParse(hn.DisplayName, out validIpAddress);
                            if (valid)
                            {
                                string ipAddress = validIpAddress.ToString();
                                ipAddresses.Add(ipAddress);
                            }
                        }
                    }
                }

                return ipAddresses.FirstOrDefault();
            }

            return "Not Connected";
        }

        public List<IPAddress> GetListOfLocalIpAddresses(string iPAddress)
        {
            //TODO: dodaæ mozliwosc skoanowania zakresów dla innych podsieci
            //iPAddress = "192.168.2.10";

            List<IPAddress> LocalNetworkIpList = new List<IPAddress>();

            string stripedIp = iPAddress.Substring(0, iPAddress.LastIndexOf(".", StringComparison.Ordinal) + 1);

            byte[,] range = IPRanger.ParseRange(stripedIp + 1 + "-254");

            foreach (IPAddress addr in IPRanger.Enumerate(range))
            {
                if(iPAddress != addr.ToString()) //ingnore iPAddress of host Ip
                LocalNetworkIpList.Add(addr);
            }

            return LocalNetworkIpList;
        }

        public static class IPRanger
        {
            public static byte[,] ParseRange(string str)
            {
                if (string.IsNullOrEmpty(str)) throw new ArgumentException("str");

                string[] partStr = str.Split('.');
                if (partStr.Length != 4) throw new FormatException();

                byte[,] range = new byte[4, 2];
                for (int i = 0; i < 4; i++)
                {
                    string[] rangeStr = partStr[i].Split('-');
                    if (rangeStr.Length > 2) throw new FormatException();

                    range[i, 0] = byte.Parse(rangeStr[0]);
                    range[i, 1] = byte.Parse(rangeStr[Math.Min(rangeStr.Length - 1, 1)]);

                    // Remove this to allow ranges to wrap around.
                    // For example: 254-4 = 254, 255, 0, 1, 2, 3, 4
                    if (range[i, 1] < range[i, 0]) throw new FormatException();
                }

                return range;
            }

            #region Enumerate Addresses
            public static IEnumerable<IPAddress> Enumerate(byte[,] range)
            {
                if (range.GetLength(0) != 4) throw new ArgumentException("range");
                if (range.GetLength(1) != 2) throw new ArgumentException("range");

                for (byte a = range[0, 0]; a != (byte) (range[0, 1] + 1); a++)
                {
                    for (byte b = range[1, 0]; b != (byte) (range[1, 1] + 1); b++)
                    {
                        for (byte c = range[2, 0]; c != (byte) (range[2, 1] + 1); c++)
                        {
                            for (byte d = range[3, 0]; d != (byte) (range[3, 1] + 1); d++)
                            {
                                yield return new IPAddress(new byte[] {a, b, c, d});
                            }
                        }
                    }
                }
            }
            #endregion
        }

        
    }

    public class AboutPartViewModel : Mvvm.ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var ver = Windows.ApplicationModel.Package.Current.Id.Version;
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." +
                       ver.Revision.ToString();
            }
        }

        public Uri RateMe => new Uri("http://bing.com");
    }
}


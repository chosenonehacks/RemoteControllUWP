using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
//using Windows.Web.Http;
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

        
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (Views.Shell.Instance.IsBusyVisible)
            {
                Views.Shell.SetBusyVisibility(Visibility.Collapsed);
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }
    }

    public class SettingsPartViewModel : Mvvm.ViewModelBase
    {
        private Services.SettingsServices.SettingsService _settings;
        private ISettingsManager _manager;
        private DialogService _dialog;
        private ResourceLoader _loader;



        public SettingsPartViewModel()
        {
            
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
                _manager = new LocalSettingsManager();
                _loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            }
            
            _BusyText = _loader.GetString("WaitMessage");
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

        private List<string> _listOfScannedIpAddresse;
        public List<string> ListOfScannedIpAddresses
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
                    _scanNetworkCommand = new DelegateCommand(async () => { await ScanLocalNetwork(); });
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
                    _checkIpAddressCommand = new DelegateCommand<string>(async (s) =>
                    {
                       await CheckIpAddressAsync(s);
                    });
                }
                return _checkIpAddressCommand;
            }
        }

        private string _BusyText;
        public string BusyText
        {
            get { return _BusyText; }
            set { Set(ref _BusyText, value); }
        }

        //private bool _IsShowingBusy;
        //public bool IsShowingBusy
        //{
        //    get { return _IsShowingBusy; }
        //    set { Set(ref _IsShowingBusy, value); }
        //}

        public void ShowBusy()
        {
            Views.Shell.SetBusyVisibility(Visibility.Visible, _BusyText);
        }

        public void HideBusy()
        {
            Views.Shell.SetBusyVisibility(Visibility.Collapsed);
        }

        private async Task CheckIpAddressAsync(string ipAddressToCheck)
        {
            //TODO:validate IP
            if (IsValidIp(ipAddressToCheck))
            {
                //TODO:send http request with answer
                
                    ShowBusy();
                    bool validAddress = await SendHttpRequestAsync(ipAddressToCheck);
                   

                if (validAddress)
                {
                    HideBusy();
                    await ShowDialogAsync("OkIP");
                }
                else
                {
                    HideBusy();
                    await ShowDialogAsync("BadIP");
                    //await _dialog.ShowAsync("Nie mogę połączyć do wskazanego adresu IP, spróbuj inny adres.", "Adres IP", new UICommand("OK"));

                }
            }
            else
            {
                await ShowDialogAsync("WrongFormatIP");
                //await _dialog.ShowAsync("Zły format adresu IP, wprowadź poprawny", "Zły Adres IP", new UICommand("OK"));
            }
            
        }

        private async Task ShowDialogAsync(string message)
        {
            _dialog = new DialogService();
            
            var messageHeader = _loader.GetString(string.Format("{0}Header", message));
            var messageContent = _loader.GetString(string.Format("{0}Content", message));
            

            await _dialog.ShowAsync(messageContent, messageHeader, new UICommand("OK"));
        }

        private async Task<bool> SendHttpRequestAsync(string ipAddressToCheck)
        {
            HttpResponseMessage reposneMsg;
            string address = String.Empty;
            

            address = "http://" + ipAddressToCheck + "/RemoteControl/Volume/get";
            var uri = new Uri(address, UriKind.Absolute);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(4000);
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
            if (!String.IsNullOrWhiteSpace(ipAddressToCheck))
            {
                IPAddress ipAddres;
                return IPAddress.TryParse(ipAddressToCheck, out ipAddres);
            }
            else
            {
                return false;
            }
        }

        private async Task ScanLocalNetwork()
        {
            ListOfScannedIpAddresses = new List<string>();
            string localIpAddress = GetLocalIpAddress();

            if (!String.IsNullOrEmpty(localIpAddress) && localIpAddress != "Not Connected")
            {
                List<IPAddress> IpAddressList = new List<IPAddress>();
                IpAddressList = GetListOfLocalIpAddresses(localIpAddress);

                ShowBusy();
                foreach (var ipAddress in IpAddressList)
                {
                    var validAddress = await SendHttpRequestAsync(ipAddress.ToString());

                    if (validAddress)
                    {
                        ListOfScannedIpAddresses.Add(ipAddress.ToString());
                        break;
                    }
                }
                #region Task.WhenAll
                //Task<bool>[] tasks = new Task<bool>[IpAddressList.Count];
                //for (int i = 0; i < IpAddressList.Count; i++)
                //{
                //    Task<bool> task = SendHttpRequestAsync(IpAddressList[i].ToString());
                //    tasks[i] = task;
                //}
                //await Task.WhenAll(tasks);

                //for (int i = 0; i < tasks.Length; i++)
                //{
                //    if (tasks[i].Result)
                //    {
                //        ListOfScannedIpAddresses.Add(IpAddressList[i].ToString());
                //    }
                //}
                #endregion

                IsIpListVisible = true;
                
                HideBusy();
                if (!ListOfScannedIpAddresses.Any())
                {
                    await ShowDialogAsync("WrongNetwork");
                    //await
                    //    _dialog.ShowAsync("Nie mogę znaleść dekodera w tej sieci. Czy Twój dekoder jest w tej samej sieci?",
                    //        "Wrong Network", new UICommand("OK"));
                    IsIpListVisible = false;
                }
            }
            else
            {
                _dialog = new DialogService();
                await ShowDialogAsync("NoNetwork");
                //await
                //    _dialog.ShowAsync("Nie jesteś połaczony z żadną siecią.",
                //        "Brak połączenia z siecią.", new UICommand("OK"));
                IsIpListVisible = false;
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
                            hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71) // An IEEE 802.11 wireless network interface.
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
                //TODO: maybe change how to chose if klas C network 192... then chose that one (in case more NICs which are active
                //TODO: or maybe don't return LastOrDefault but return list and iterate one after another
                //[TW]11/21/2015 - changed to return IP only that starts with 192.
                return ipAddresses.FindLast(ip => ip.StartsWith("192."));
            }

            return "Not Connected";
        }

        public List<IPAddress> GetListOfLocalIpAddresses(string iPAddress)
        {

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
                if (string.IsNullOrEmpty(str)) throw new ArgumentException("IP String null in ParseRange");

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

        public string Description => Windows.ApplicationModel.Package.Current.Description;

        public string Version
        {
            get
            {
                var ver = Windows.ApplicationModel.Package.Current.Id.Version;
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." +
                       ver.Revision.ToString();
            }
        }

        //TODO: Add store address in future.
        public Uri RateMe => new Uri("https://www.microsoft.com/store/apps/9nblggh6gxzx");
    }
}


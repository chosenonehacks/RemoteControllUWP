using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RemoteController.Services.SettingsServiceMyImplementation;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class SettingsPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : Mvvm.ViewModelBase
    {
        private Services.SettingsServices.SettingsService _settings;
        private ISettingsManager _manager;

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
            SelectedIpAddress = listView.SelectedItem.ToString();
            IpAddress = SelectedIpAddress;
            IsIpListVisible = false;
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

        private void CheckIpAddress(string ipAddressToCheck)
        {
            //TODO:validate IP

            //TODO:send http request with answer

            //TODO:dialog success
        }

        private void ScanLocalNetwork()
        {
            string localIpAddress = GetLocalIpAddress();

            if (!string.IsNullOrEmpty(localIpAddress))
            {
                List<IPAddress> IpAddressList = new List<IPAddress>();
                IpAddressList = GetListOfLocalIpAddresses(localIpAddress);
                ListOfScannedIpAddresses = new ObservableCollection<string>();

                foreach (var ipAddress in IpAddressList)
                {
                    //TODO: send http request to given address and check for respond

                    ListOfScannedIpAddresses.Add(ipAddress.ToString());
                }

                IsIpListVisible = true;
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


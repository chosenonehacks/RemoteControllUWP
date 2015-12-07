using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using RemoteController.Models;
using RemoteController.Services.SettingsServiceMyImplementation;

namespace RemoteController.ViewModels
{
    public class ListPageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private readonly ISettingsManager _manager;
        private readonly Services.RemoteController.RemoteController _remoteController;

        public ListPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
                _remoteController = new Services.RemoteController.RemoteController(IpAddress);
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

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);
    }
}

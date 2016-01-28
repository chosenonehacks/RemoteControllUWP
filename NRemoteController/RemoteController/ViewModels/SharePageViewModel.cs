using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Navigation;
using RemoteController.Models;
using RemoteController.Services.DialogService;
using RemoteController.Services.SettingsServiceMyImplementation;
using RemoteController.Views;
using Template10.Mvvm;

namespace RemoteController.ViewModels
{
    public class SharePageViewModel : RemoteController.Mvvm.ViewModelBase
    {
        private DataTransferManager dataTransferManager;
        private readonly ISettingsManager _manager;
        private DialogService _dialog;
        private readonly Services.RemoteController.RemoteController _remoteController;
        private ResourceLoader _loader;
        private TvChannels currentTvChannel;


        public SharePageViewModel()
        {
            

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _manager = new LocalSettingsManager();
                _remoteController = new Services.RemoteController.RemoteController(IpAddress);
                _loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            }
        }

        private string _shareText;
        public string ShareText
        {
            get { return _shareText; }
            set { Set(ref _shareText, value); }
        }

        private string _shareTarget;
        public string ShareTarget
        {
            get { return _shareTarget; }
            set { Set(ref _shareTarget, value); }
        }

        private bool _startedSharing;
        public bool StartedSharing
        {
            get { return _startedSharing; }
            set { Set(ref _startedSharing, value); }
        }

        public string IpAddress => _manager.Load<string>("IpSetting", String.Empty);

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += SharePage_DataRequested;
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().TargetApplicationChosen += SharePageViewModel_TargetApplicationChosen;
            

            currentTvChannel = await _remoteController.GetCurrentChanellInfoAsync();

            var messageTop = _loader.GetString("ShareMessageTop");
            var messageBack = _loader.GetString("ShareMessageBack");

            ShareText = messageTop + currentTvChannel.Name + messageBack;
        }

        private void SharePageViewModel_TargetApplicationChosen(DataTransferManager sender, TargetApplicationChosenEventArgs args)
        {
            ShareTarget = args.ApplicationName;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested -= SharePage_DataRequested;
            return base.OnNavigatedFromAsync(state, suspending);
        }

        void SharePage_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(ShareText))
            {
                args.Request.Data.SetText(ShareText);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
                
            }
            else
            {
                args.Request.FailWithDisplayText("Nothing to share");
            }
        }

        private DelegateCommand _shareCommand;
        public DelegateCommand ShareCommand
        {
            get { return _shareCommand ?? (_shareCommand = new DelegateCommand(Share)); }
        }

        private void Share()
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            
        }



    }
}
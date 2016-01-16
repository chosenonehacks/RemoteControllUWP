using System;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using RemoteController.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Common;

namespace RemoteController
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        ISettingsService _settings;

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;
        }

        // runs even if restored from state
        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // setup hamburger shell
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            return Task.FromResult<object>(null);
        }

        // runs only when not restored from state
        //public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        //{
        //    await Task.Delay(50);
        //    NavigationService.Navigate(typeof(Views.MainPage));
        //}

        //[TW] 1/16/2016 - Better fix for returning for suspending before release of 1.0.9 of template 10 
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // navigate to first page
            var nav = WindowWrapper.ActiveWrappers.FirstOrDefault(wrapper => object.ReferenceEquals(wrapper.Window, Window.Current)).NavigationServices;
            if (nav.Count > 1)
            {
                nav.Remove(nav[0]);
            }

            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        //First fix to suspending / resume bug 
        //public static void ClearNavigationServices(Window window)
        //{
        //    var wrapperToRemove = WindowWrapper.ActiveWrappers.FirstOrDefault(wrapper => object.ReferenceEquals(wrapper.Window, window));
        //    if (wrapperToRemove != null)
        //    {
        //        wrapperToRemove.NavigationServices.Clear();
        //    }
        //}

        //public override Task OnSuspendingAsync(object s, SuspendingEventArgs e)
        //{
        //    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
        //    {
        //        ClearNavigationServices(Window.Current);
        //    }
        //    return base.OnSuspendingAsync(s, e);
        //}
    }
}


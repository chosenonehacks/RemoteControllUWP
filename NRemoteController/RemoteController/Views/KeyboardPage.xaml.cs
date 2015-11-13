using RemoteController.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace RemoteController.Views
{
    public sealed partial class KeyboardPage : Page
    {
        public KeyboardPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public KeyboardPageViewModel ViewModel => DataContext as KeyboardPageViewModel;
        
    }
}


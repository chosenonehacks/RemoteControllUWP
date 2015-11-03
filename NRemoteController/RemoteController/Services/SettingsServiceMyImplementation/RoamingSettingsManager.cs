using Windows.Storage;

namespace RemoteController.Services.SettingsServiceMyImplementation
{
    public class RoamingSettingsManager : SettingsManager
    {
        public RoamingSettingsManager()
            : base(ApplicationData.Current.RoamingSettings)
        {
        }
    }
}
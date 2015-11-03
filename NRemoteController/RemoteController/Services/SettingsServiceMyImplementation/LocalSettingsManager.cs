using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RemoteController.Services.SettingsServiceMyImplementation
{
    public class LocalSettingsManager : SettingsManager
    {
        public LocalSettingsManager()
            : base(ApplicationData.Current.LocalSettings)
        {
        }
    }
}

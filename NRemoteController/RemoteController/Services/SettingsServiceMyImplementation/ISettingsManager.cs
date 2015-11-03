using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController.Services.SettingsServiceMyImplementation
{
    public interface ISettingsManager
    {
        T Load<T>(string key);
        T Load<T>(string key, T defaultValue);
        object Load(string key);
        void Save(string key, object value);
        void RemoveSetting(string key);
        void ClearAllSettings();
    }
}

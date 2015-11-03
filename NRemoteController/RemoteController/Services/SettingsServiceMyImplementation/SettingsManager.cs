using Windows.Storage;

namespace RemoteController.Services.SettingsServiceMyImplementation
{
    public abstract class SettingsManager : ISettingsManager
    {
        protected ApplicationDataContainer storageContainer = null;

        public SettingsManager(ApplicationDataContainer storageContainer)
        {
            this.storageContainer = storageContainer;
        }

        public T Load<T>(string key)
        {
            object ret = Load(key);
            if (ret == null)
            {
                return default(T);
            }
            return (T)ret;
        }

        public T Load<T>(string key, T defaultValue)
        {
            object ret = Load(key);
            if (ret == null)
            {
                return defaultValue;
            }
            return (T)ret;
        }

        public object Load(string key)
        {
            object output = null;
            storageContainer.Values.TryGetValue(key, out output);
            return output;
        }

        public void Save(string key, object value)
        {
            if (storageContainer.Values.ContainsKey(key))
            {
                storageContainer.Values[key] = value;
            }
            else
            {
                storageContainer.Values.Add(key, value);
            }
        }

        public void RemoveSetting(string key)
        {
            if (storageContainer.Values[key] != null)
            {
                storageContainer.Values.Remove(key);
            }
        }

        public virtual void ClearAllSettings()
        {
            storageContainer.Values.Clear();
        }
    }
}
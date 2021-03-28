using System;
using System.IO;
using Edimsha.WPF.Settings;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : ISaveSettingsService
    {
        private const string SETTINGS_FILE = @"Settings/appsettings.json";

        public void SaveConfigurationSettings<T>(string settingName, T value)
        {
            Config newconfig;
            
            using (var settings = File.OpenText(SETTINGS_FILE))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                var propertyInfo = config?.GetType().GetProperty(settingName);
                if (propertyInfo != null)
                    propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);

                newconfig = config;
            }
            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(newconfig, Formatting.Indented));
        }
    }
}
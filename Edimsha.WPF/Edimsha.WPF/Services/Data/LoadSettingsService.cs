using System;
using Edimsha.WPF.Settings;
using Newtonsoft.Json;
using static System.IO.File;

namespace Edimsha.WPF.Services.Data
{
    public class LoadSettingsService : SettingsService, ILoadSettingsService
    {
        public LoadSettingsService(ConfigPaths settingsPath) : base(settingsPath)
        {
        }

        public T LoadConfigurationSetting<T>(string settingName)
        {
            using (var settings = OpenText(SettingsPath))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
            }

            throw new Exception($"LoadConfigurationSetting no ha encontrado {settingName}");
        }
    }
}
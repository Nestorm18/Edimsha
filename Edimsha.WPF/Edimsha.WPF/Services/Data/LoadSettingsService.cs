using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Edimsha.WPF.Settings;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class LoadSettingsService : SettingsService, ILoadSettingsService
    {
        public LoadSettingsService(ConfigPaths settingsPath) : base(settingsPath)
        {
        }

        public T LoadConfigurationSetting<T>(string settingName)
        {
            using (var settings = File.OpenText(SettingsPath))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
            }

            throw new Exception($"LoadConfigurationSetting no ha encontrado {settingName}");
        }

        public ObservableCollection<string> LoadPathsListview(ObservableCollection<string> paths)
        {
            using (var pathsJson = File.OpenText(EditorPathsJson))
            {
                var serializer = new JsonSerializer();
                var list = (List<string>) serializer.Deserialize(pathsJson, typeof(List<string>));

                paths.Clear();
                list?.ForEach(paths.Add);

                return paths;
            }
        }
    }
}
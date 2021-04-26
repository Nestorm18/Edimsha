using System;
using System.Collections.Generic;
using System.IO;
using Edimsha.WPF.Models;
using Edimsha.WPF.Settings;
using Edimsha.WPF.State.Navigators;
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

        public List<string> LoadPathsListview(ViewType type)
        {
            var file = type switch
            {
                ViewType.Editor => EditorPathsJson,
                ViewType.Conversor => ConversorPathsJson,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            using (var pathsJson = File.OpenText(file))
            {
                var serializer = new JsonSerializer();
                return (List<string>) serializer.Deserialize(pathsJson, typeof(List<string>));
            }
        }

        public IEnumerable<Resolution> LoadResolutions()
        {
            if (!File.Exists(ResolutionsJson)) throw new Exception($"LoadResolutions no ha encontrado archivo");

            var resolutions = File.ReadAllText(ResolutionsJson);
            return JsonConvert.DeserializeObject<List<Resolution>>(resolutions);
        }
    }
}
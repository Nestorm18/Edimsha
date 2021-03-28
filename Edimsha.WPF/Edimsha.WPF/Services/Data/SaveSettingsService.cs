using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edimsha.WPF.Settings;
using Edimsha.WPF.State.Navigators;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : SettingsService, ISaveSettingsService
    {
        public SaveSettingsService(ConfigPaths settingsPath) : base(settingsPath)
        {
        }

        public async Task<bool> SaveConfigurationSettings<T>(string settingName, T value)
        {
            Config newconfig;

            using (var settings = File.OpenText(SettingsPath))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                var propertyInfo = config?.GetType().GetProperty(settingName);
                if (propertyInfo != null)
                    propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);

                newconfig = config;
            }

            await File.WriteAllTextAsync(SettingsPath, JsonConvert.SerializeObject(newconfig, Formatting.Indented));

            return true;
        }

        public async Task<bool> SavePathsListview(IEnumerable<string> values, ViewType viewType)
        {
            string pathFile;

            switch (viewType)
            {
                case ViewType.Editor:
                    pathFile = EditorPathsJson;
                    break;
                case ViewType.Conversor:
                    pathFile = ConversorPathsJson;
                    break;
                default:
                    throw new Exception("SavePathsListview viewType no encontrado");
            }

            await File.WriteAllTextAsync(pathFile, JsonConvert.SerializeObject(values.ToList(), Formatting.Indented));

            return true;
        }
    }
}
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
    public class SaveSettingsService : ISaveSettingsService
    {
        private readonly string _settingsPath;
        private readonly string _editorPathsJson;
        private readonly string _conversorPathsJson;

        public SaveSettingsService(ConfigPaths settingsPath)
        {
            _settingsPath = settingsPath.SettingsFile;
            _editorPathsJson = settingsPath.EditorPathsJson;
            _conversorPathsJson = settingsPath.ConversorPathsJson;
        }

        public async Task<bool> SaveConfigurationSettings<T>(string settingName, T value)
        {
            Config newconfig;

            using (var settings = File.OpenText(_settingsPath))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                var propertyInfo = config?.GetType().GetProperty(settingName);
                if (propertyInfo != null)
                    propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);

                newconfig = config;
            }

            await File.WriteAllTextAsync(_settingsPath, JsonConvert.SerializeObject(newconfig, Formatting.Indented));

            return true;
        }

        public async Task<bool> SavePathsListview(IEnumerable<string> values, ViewType viewType)
        {
            string pathFile;
            
            switch (viewType)
            {
                case ViewType.Editor:
                    pathFile = _editorPathsJson;
                    break;
                case ViewType.Conversor:
                    pathFile = _conversorPathsJson;
                    break;
                default:
                    throw new Exception("SavePathsListview viewType no encontrado");
            }
            
            List<string> savedpaths;
            using (var file = File.OpenText(pathFile))
            {
                var serializer = new JsonSerializer();
                savedpaths = (List<string>) serializer.Deserialize(file, typeof(List<string>));
            }

            var filePathsDistinct = savedpaths!.Concat(values.ToList()).Distinct().ToList();
            
            await File.WriteAllTextAsync(pathFile, JsonConvert.SerializeObject(filePathsDistinct, Formatting.Indented));

            return true;
        }
    }
}
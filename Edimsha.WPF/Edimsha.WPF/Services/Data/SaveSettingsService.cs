using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Models;
using Edimsha.WPF.Settings;
using Edimsha.WPF.State.Navigators;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : SettingsService, ISaveSettingsService
    {
        public SaveSettingsService(ConfigPaths settingsPath) : base(settingsPath)
        {
            Logger.Log("Constructor");
        }

        public async Task<bool> SaveConfigurationSettings<T>(string settingName, T value)
        {
            // Update Config.cs file when you add new setting to json
            Config newconfig;

            Logger.Log($"SettingName: {settingName}, Value: {value}");
            using (var settings = File.OpenText(SettingsPath))
            {
                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings, typeof(Config));

                var propertyInfo = config?.GetType().GetProperty(settingName);
                if (propertyInfo != null)
                    propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                else
                    return false;

                newconfig = config;
            }

            Logger.Log("Writing file...");
            await File.WriteAllTextAsync(SettingsPath, JsonConvert.SerializeObject(newconfig, Formatting.Indented));

            return true;
        }

        public async Task<bool> SavePathsListview(IEnumerable<string> values, ViewType viewType)
        {
            Logger.Log($"Values: {values}, ViewType: {viewType}");
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

            Logger.Log("Writing file...");
            await File.WriteAllTextAsync(pathFile, JsonConvert.SerializeObject(values.ToList(), Formatting.Indented));

            return true;
        }

        public async Task<bool> SaveResolutions(IEnumerable<Resolution> resolutions)
        {
            Logger.Log($"Resolutions {resolutions}");
            if (!File.Exists(ResolutionsJson)) throw new Exception($"SaveResolutions no ha encontrado archivo");

            var formatedJson = JsonConvert.SerializeObject(resolutions, Formatting.Indented);
            
            Logger.Log("Writing file...");
            await File.WriteAllTextAsync(ResolutionsJson, formatedJson);

            return true;
        }
    }
}
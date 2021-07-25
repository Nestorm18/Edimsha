using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.State.Navigators;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : SettingsService, ISaveSettingsService
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        public SaveSettingsService(ConfigPaths settingsPath) : base(settingsPath)
        {
            
        }

        public async Task<bool> SaveConfigurationSettings<T>(ViewType type, string settingName, T value)
        {
            // Update Config.cs file when you add new setting to json
            ConfigEditor newconfig;

            switch (type)
            {
                case ViewType.Editor:
                    _logger.Info($"Editor SettingName: {settingName}, Value: {value}");
                    using (var settings = File.OpenText(SettingsEditor))
                    {
                        var serializer = new JsonSerializer();
                        var config = (ConfigEditor) serializer.Deserialize(settings, typeof(ConfigEditor));

                        var propertyInfo = config?.GetType().GetProperty(settingName);
                        if (propertyInfo != null)
                            propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        else
                            return false;

                        newconfig = config;
                    }

                    _logger.Info("Writing file...");
                    await File.WriteAllTextAsync(SettingsEditor, JsonConvert.SerializeObject(newconfig, Formatting.Indented));
                    break;
                case ViewType.Converter:
                    _logger.Info($"Conversor SettingName: {settingName} C, Value: {value}");
                    using (var settings = File.OpenText(SettingsConversor))
                    {
                        var serializer = new JsonSerializer();
                        var config = (ConfigEditor) serializer.Deserialize(settings, typeof(ConfigEditor));

                        var propertyInfo = config?.GetType().GetProperty(settingName);
                        if (propertyInfo != null)
                            propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        else
                            return false;

                        newconfig = config;
                    }

                    _logger.Info("Writing file...");
                    await File.WriteAllTextAsync(SettingsConversor, JsonConvert.SerializeObject(newconfig, Formatting.Indented));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return true;
        }

        public bool SavePaths(IEnumerable<string> values, ViewType viewType)
        {
            _logger.Info($"Values: {values}, ViewType: {viewType}");
            string pathFile;

            switch (viewType)
            {
                case ViewType.Editor:
                    pathFile = EditorPathsJson;
                    break;
                case ViewType.Converter:
                    pathFile = ConversorPathsJson;
                    break;
                default:
                    throw new Exception("SavePathsListview viewType no encontrado");
            }

            _logger.Info("Writing file...");
            File.WriteAllTextAsync(pathFile, JsonConvert.SerializeObject(values.ToList(), Formatting.Indented));
            _logger.Info("File done!");

            return true;
        }

        public async void SaveResolutions(IEnumerable<Resolution> resolutions)
        {
            _logger.Info($"Resolutions {resolutions}");
            if (!File.Exists(ResolutionsJson)) throw new Exception($"SaveResolutions no ha encontrado archivo");

            var formatedJson = JsonConvert.SerializeObject(resolutions, Formatting.Indented);

            _logger.Info("Writing file...");
            await File.WriteAllTextAsync(ResolutionsJson, formatedJson);
        }
    }
}
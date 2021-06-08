using System;
using System.Collections.Generic;
using System.IO;
using Edimsha.Core.Logging.Core;
using Edimsha.Core.Logging.Implementation;
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
            Logger.Log("Constructor loaded...", LogLevel.Debug);
        }

        public T LoadConfigurationSetting<T>(ViewType type, string settingName)
        {
            try
            {
                switch (type)
                {
                    case ViewType.Editor:
                        using (var settings = File.OpenText(SettingsEditor))
                        {
                            Logger.Log($"settingName: {settingName}", LogLevel.Debug);

                            var serializer = new JsonSerializer();
                            var config = (Config) serializer.Deserialize(settings, typeof(Config));

                            if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
                        }
                        break;
                    case ViewType.Conversor:
                        using (var settings = File.OpenText(SettingsConversor))
                        {
                            Logger.Log($"settingName: {settingName}", LogLevel.Debug);

                            var serializer = new JsonSerializer();
                            var config = (Config) serializer.Deserialize(settings, typeof(Config));

                            if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
                        }
                        break;
                    default:
                        var ex = new ArgumentOutOfRangeException(nameof(type), type, null);
                        Logger.Log(ex.StackTrace, LogLevel.Error);
                        throw ex;
                }
                
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, LogLevel.Error);
                throw;
            }

            return (T) new object();
        }

        public List<string> LoadPathsListview(ViewType type)
        {
            Logger.Log($"ViewType: {type}", LogLevel.Debug);

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
            Logger.Log("Loading...", LogLevel.Debug);

            if (!File.Exists(ResolutionsJson)) throw new Exception($"LoadResolutions no ha encontrado archivo");

            var resolutions = File.ReadAllText(ResolutionsJson);
            return JsonConvert.DeserializeObject<List<Resolution>>(resolutions);
        }

        public Config GetConfigFormViewType(ViewType type)
        {
            try
            {
                switch (type)
                {
                    case ViewType.Editor:
                        using (var settings = File.OpenText(SettingsEditor))
                        {
                            Logger.Log($"Obtaining all settings editor", LogLevel.Debug);

                            var serializer = new JsonSerializer();
                            return (Config) serializer.Deserialize(settings, typeof(Config));
                        }
                    case ViewType.Conversor:
                        using (var settings = File.OpenText(SettingsConversor))
                        {
                            Logger.Log($"Obtaining all settings converter", LogLevel.Debug);

                            var serializer = new JsonSerializer();
                            return (Config) serializer.Deserialize(settings, typeof(Config));
                        }
                    default:
                        var ex = new ArgumentOutOfRangeException(nameof(type), type, null);
                        Logger.Log(ex.StackTrace, LogLevel.Error);
                        throw ex;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, LogLevel.Error);
                throw;
            }
        }
    }
}
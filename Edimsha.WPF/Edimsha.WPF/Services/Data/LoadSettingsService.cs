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
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Log($"settingName: {settingName}", LogLevel.Debug);

                var serializer = new JsonSerializer();
                var config = (Config) serializer.Deserialize(settings!, typeof(Config));

                if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, LogLevel.Error);
                throw;
            }
            finally
            {
                settings.Close();
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
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Log("Obtaining all settings editor", LogLevel.Debug);

                var serializer = new JsonSerializer();
                return (Config) serializer.Deserialize(settings, typeof(Config));
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, LogLevel.Error);
                throw;
            }
            finally
            {
                settings.Close();
            }
        }

        private StreamReader GetSettingFileWithViewType(ViewType type)
        {
            StreamReader settings = null;

            try
            {
                switch (type)
                {
                    case ViewType.Editor:
                    {
                        settings = File.OpenText(SettingsEditor);
                        break;
                    }
                    case ViewType.Conversor:
                    {
                        settings = File.OpenText(SettingsConversor);
                        break;
                    }
                    default:
                        ArgumentExceptionLoggedAndThrowed(type);
                        break;
                }
            }
            catch (Exception e)
            {
                ArgumentExceptionLoggedAndThrowed(type);
            }

            return settings;
        }

        private static void ArgumentExceptionLoggedAndThrowed(ViewType type)
        {
            var ex = new ArgumentOutOfRangeException(nameof(type), type, null);
            Logger.Log(ex.StackTrace, LogLevel.Error);
            throw ex;
        }
    }
}
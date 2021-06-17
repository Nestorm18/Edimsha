using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edimsha.Core.Logging.Core;
using Edimsha.Core.Logging.Implementation;
using Edimsha.Core.Settings;
using Edimsha.WPF.Models;
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
                var config = (ConfigEditor) serializer.Deserialize(settings!, typeof(ConfigEditor));

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

        public IEnumerable<string> GetSavedPaths(ViewType type)
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

        public ConfigEditor GetConfigFormViewType(ViewType type)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Log("Obtaining all settings editor", LogLevel.Debug);

                var serializer = new JsonSerializer();
                return (ConfigEditor) serializer.Deserialize(settings, typeof(ConfigEditor));
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

        public bool StillPathsSameFromLastSession(ViewType type)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Log("Obtaining last session paths", LogLevel.Debug);

                var paths = GetSavedPaths(type);

                return paths.All(File.Exists);
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

        public IEnumerable<string> GetPathChanges(ViewType type)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Log("Gettings last session paths differences", LogLevel.Debug);

                var changes = GetSavedPaths(type).Where(path => !File.Exists(path)).ToList();

                return changes.Count == 0 ? null : changes;
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
            catch (Exception)
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
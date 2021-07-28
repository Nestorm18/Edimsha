using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.State.Navigators;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class LoadSettingsService : ILoadSettingsService
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IOptions<ConfigPaths> _options;

        public LoadSettingsService(IOptions<ConfigPaths> options)
        {
            Logger.Info("Constructor loaded...");
            _options = options;
        }

        /// <inheritdoc />
        public T LoadConfigurationSetting<T, TClass>(string settingName, string filePath)
        {
            try
            {
                Logger.Info($"settingName: {settingName}, using: {filePath}");

                using var settings = File.OpenText(Path.GetFullPath(filePath));
                
                var serializer = new JsonSerializer();
                var config = (TClass) serializer.Deserialize(settings!, typeof(TClass));

                if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
                throw;
            }

            return default;
        }

        public IEnumerable<string> GetSavedPaths(ViewType type)
        {
            Logger.Info($"ViewType: {type}");

            var file = type switch
            {
                ViewType.Editor => _options.Value.EditorPaths,
                ViewType.Converter => _options.Value.ConversorPaths,
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
            Logger.Info("Loading...");

            if (!File.Exists(_options.Value.Resolutions)) throw new Exception($"LoadResolutions no ha encontrado archivo");

            var resolutions = File.ReadAllText(_options.Value.Resolutions);
            return JsonConvert.DeserializeObject<List<Resolution>>(resolutions);
        }

        public ConfigEditor GetConfigFormViewType(ViewType type)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                Logger.Info("Obtaining all settings editor");

                var serializer = new JsonSerializer();
                return (ConfigEditor) serializer.Deserialize(settings, typeof(ConfigEditor));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
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
                Logger.Info("Obtaining last session paths");

                var paths = GetSavedPaths(type);

                return paths.All(File.Exists);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
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
                Logger.Info("Gettings last session paths differences");

                var changes = GetSavedPaths(type).Where(path => !File.Exists(path)).ToList();

                return changes.Count == 0 ? null : changes;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
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
                        settings = File.OpenText(_options.Value.SettingsEditor);
                        break;
                    case ViewType.Converter:
                        settings = File.OpenText(_options.Value.SettingsConversor);
                        break;
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
            Logger.Error(ex.StackTrace, "Stopped program because of exception");
            throw ex;
        }
    }
}
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
       
        private IOptions<ConfigPaths> _options;

        public LoadSettingsService(IOptions<ConfigPaths> options)
        {
            _logger.Info("Constructor loaded...");
            _options = options;
        }

        public T LoadConfigurationSetting<T>(ViewType type, string settingName)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                _logger.Info($"settingName: {settingName}");

                var serializer = new JsonSerializer();
                var config = (ConfigEditor) serializer.Deserialize(settings!, typeof(ConfigEditor));

                if (config != null) return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
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
            _logger.Info($"ViewType: {type}");

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
            _logger.Info("Loading...");

            if (!File.Exists(_options.Value.Resolutions)) throw new Exception($"LoadResolutions no ha encontrado archivo");

            var resolutions = File.ReadAllText(_options.Value.Resolutions);
            return JsonConvert.DeserializeObject<List<Resolution>>(resolutions);
        }

        public ConfigEditor GetConfigFormViewType(ViewType type)
        {
            var settings = GetSettingFileWithViewType(type);

            try
            {
                _logger.Info("Obtaining all settings editor");

                var serializer = new JsonSerializer();
                return (ConfigEditor) serializer.Deserialize(settings, typeof(ConfigEditor));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
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
                _logger.Info("Obtaining last session paths");

                var paths = GetSavedPaths(type);

                return paths.All(File.Exists);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
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
                _logger.Info("Gettings last session paths differences");

                var changes = GetSavedPaths(type).Where(path => !File.Exists(path)).ToList();

                return changes.Count == 0 ? null : changes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
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
            _logger.Error(ex.StackTrace, "Stopped program because of exception");
            throw ex;
        }
    }
}
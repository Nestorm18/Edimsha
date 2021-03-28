using System;
using System.IO;
using System.Threading.Tasks;
using Edimsha.WPF.Settings;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : ISaveSettingsService
    {
        private readonly string _connectionString;

        public SaveSettingsService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> SaveConfigurationSettings<T>(string settingName, T value)
        {
            try
            {
                Config newconfig;

                using (var settings = File.OpenText(_connectionString))
                {
                    var serializer = new JsonSerializer();
                    var config = (Config) serializer.Deserialize(settings, typeof(Config));

                    var propertyInfo = config?.GetType().GetProperty(settingName);
                    if (propertyInfo != null)
                        propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);

                    newconfig = config;
                }

                File.WriteAllText(_connectionString, JsonConvert.SerializeObject(newconfig, Formatting.Indented));

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
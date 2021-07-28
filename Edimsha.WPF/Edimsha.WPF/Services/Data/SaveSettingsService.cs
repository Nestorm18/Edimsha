using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class SaveSettingsService : ISaveSettingsService
    {
        public async Task<bool> SaveConfigurationSettings<T, TClass>(string settingName, T value, string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            using var settings = File.OpenText(fullPath);

            var serializer = new JsonSerializer();
            var config = (TClass) serializer.Deserialize(settings, typeof(TClass));

            var propertyInfo = config?.GetType().GetProperty(settingName);
            
            if (propertyInfo != null)
                propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            else
                return false;

            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            
            return true;
        }

        public async Task<bool> SaveListToFile<T>(IEnumerable<T> list, string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(list, Formatting.Indented));

            return true;
        }
    }
}
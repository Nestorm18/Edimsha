using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Edimsha.WPF.Services.Data
{
    public class LoadSettingsService : ILoadSettingsService
    {
        /// <inheritdoc />
        public T LoadConfigurationSetting<T, TClass>(string settingName, string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            using var settings = File.OpenText(fullPath);

            var serializer = new JsonSerializer();
            
            var config = (TClass) serializer.Deserialize(settings!, typeof(TClass));

            if (config != null)
                return (T) config.GetType().GetProperty(settingName)?.GetValue(config, null)
                       ?? throw new ArgumentException("Setting not found.");

            return default;
        }
        
        /// <inheritdoc />
        public TClass GetFullConfig<TClass>(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            using var config = File.OpenText(fullPath);

            var serializer = new JsonSerializer();

            return (TClass) serializer.Deserialize(config, typeof(TClass));
        }

        /// <inheritdoc />
        public bool StillPathsSameFromLastSession<T>(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);
           
            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            var paths = LoadConfigurationSetting<List<string>, T>("Paths", filePath);

            return paths.All(File.Exists);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPathChanges<T>(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            var changes = LoadConfigurationSetting<List<string>, T>("Paths", filePath).Where(path => !File.Exists(path)).ToList();

            return changes.Count == 0 ? null : changes;
        }
    }
}
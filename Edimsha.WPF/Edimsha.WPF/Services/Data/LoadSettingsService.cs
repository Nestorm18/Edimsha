using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edimsha.Core.Models;
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
        public IEnumerable<string> GetSavedPaths(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            using var pathsJson = File.OpenText(fullPath);
            var serializer = new JsonSerializer();

            return (List<string>) serializer.Deserialize(pathsJson, typeof(List<string>))
                   ?? throw new ArgumentException("Setting not found.");
        }

        /// <inheritdoc />
        public IEnumerable<Resolution> LoadResolutions(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            var resolutions = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Resolution>>(resolutions);
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
        public bool StillPathsSameFromLastSession(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            var paths = GetSavedPaths(filePath);

            return paths.All(File.Exists);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPathChanges(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException($"The file in {fullPath} not exist.");

            var changes = GetSavedPaths(filePath).Where(path => !File.Exists(path)).ToList();

            return changes.Count == 0 ? null : changes;
        }
    }
}
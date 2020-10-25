using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Edimsha.Storage
{
    internal class StoragePaths : Storage
    {
        public StoragePaths(string filePaths) : base(filePaths)
        {
        }

        public bool KeepSavedPreviousPaths { get; internal set; }

        public void SavePaths(List<string> paths)
        {
            if (KeepSavedPreviousPaths)
            {
                var oldPaths = GetObject<string>();
                paths.AddRange(oldPaths);

                // Remove Duplicates
                paths = paths.Distinct().ToList();
            }

            File.WriteAllText(storePath, JsonConvert.SerializeObject(paths, Formatting.Indented));
        }

        public bool StillPathsSameFromLastSession()
        {
            var paths = GetObject<string>();
            if (paths.Count > 0)
                foreach (var path in paths)
                    if (!File.Exists(path))
                        return false;
            return true;
        }

        public List<string> GetPathChanges()
        {
            var changes = new List<string>();

            foreach (var path in GetObject<string>())
                if (!File.Exists(path))
                    changes.Add(path);

            if (changes.Count == 0)
                return null;
            return changes;
        }

        public void RemoveMissingPathsFromLastSession()
        {
            var pathList = GetObject<string>();
            var save = false;

            for (var i = 0; i < pathList.Count; i++)
                if (!File.Exists(pathList[i]))
                {
                    save = true;
                    pathList[i] = "";
                }

            pathList = pathList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            if (save)
                SavePaths(pathList);
        }

        internal void RemovePath(string path)
        {
            var paths = GetObject<string>();
            paths.RemoveAll(x => x.Contains(path));

            SavePaths(paths);
        }
    }
}
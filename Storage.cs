using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;

namespace Edimsha
{
    class Storage : FilePaths
    {
        private readonly string storePath;

        public bool KeepSavedPreviousPaths { get; internal set; }

        public Storage(string filePaths)
        {
            storePath = filePaths;
        }

        public void SavePaths(List<string> paths)
        {
            if (KeepSavedPreviousPaths)
            {
                List<string> oldPaths = GetPaths();
                paths.AddRange(oldPaths);

                // Remove Duplicates
                paths = paths.Distinct().ToList();
            }
            File.WriteAllText(storePath, JsonConvert.SerializeObject(paths, Formatting.Indented));
        }

        public List<string> GetPaths()
        {
            string json = File.ReadAllText(storePath);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        public bool StillPathsSameFromLastSession()
        {
            List<string> paths = GetPaths();
            if (paths.Count > 0)
                foreach (var path in paths)
                    if (!File.Exists(path))
                        return false;
            return true;
        }

        public List<string> GetPathChanges()
        {
            List<string> changes = new List<string>();

            foreach (var path in GetPaths())
                if (!File.Exists(path))
                    changes.Add(path);

            if (changes.Count == 0)
                return null;
            else
                return changes;

        }

        public void RemoveMissingPathsFromLastSession()
        {
            List<string> pathList = GetPaths();
            bool save = false;

            for (int i = 0; i < pathList.Count; i++)
            {               
                if (!File.Exists(pathList[i]))
                {
                    save = true;
                    pathList[i] = "";
                }
            }

            pathList = pathList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            if (save)
                SavePaths(pathList);                
        }

        public void CleanFile()
        {
            File.WriteAllText(storePath, "[]");
        }

        internal void RemovePath(string path)
        {
            List<string> paths = GetPaths();
            paths.RemoveAll(x => x.Contains(path));
            
            SavePaths(paths);
        }
    }
}

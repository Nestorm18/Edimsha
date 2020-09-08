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

        public bool IsPathsStillAvailableFromLastSession(bool removeMissingPaths=false)
        {
            List<string> pathList = GetPaths();
            bool save = false;

            for (int i = 0; i < pathList.Count; i++)
            {
                if (!File.Exists(pathList[i]))
                {
                    save = true;
                    if (removeMissingPaths) pathList.RemoveAt(i);
                }
            }

            if (save)           
                if (removeMissingPaths) SavePaths(pathList);
            else           
                return true;

            return false;
        }

        public void CleanFile()
        {
            File.WriteAllText(storePath, "[]");
        }
    }
}

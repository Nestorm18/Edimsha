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
            // TODO: Comprobar si las rutas del json siguen siendo validas
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
    }
}

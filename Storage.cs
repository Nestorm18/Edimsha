using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace Edimsha
{
    class Storage : FilePaths
    {
        private readonly string storePath;

        public Storage(string filePaths)
        {
            storePath = filePaths;
        }

        public void SavePaths(List<string> paths)
        {
            File.WriteAllText(storePath, JsonConvert.SerializeObject(paths, Formatting.Indented));            
        }

        public List<string> GetPaths()
        {
            string json = File.ReadAllText(storePath);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }
    }
}

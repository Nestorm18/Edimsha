using System.Collections.Generic;
using System.IO;
using Edimsha.Edition;
using Newtonsoft.Json;

namespace Edimsha.Storage
{
    internal class Storage : FilePaths
    {
        protected readonly string storePath;

        public Storage(string filePaths)
        {
            storePath = filePaths;
        }

        public void CleanFile()
        {
            File.WriteAllText(storePath, "[]");
        }

        public List<T> GetObject<T>()
        {
            var json = File.ReadAllText(storePath);
            var jsDes = JsonConvert.DeserializeObject<List<T>>(json);

            if (jsDes != null)
                return JsonConvert.DeserializeObject<List<T>>(json);
            return new List<T>();
        }
    }
}
using Edimsha.Edition;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Edimsha.Storage
{
    class Storage : FilePaths
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
            string json = File.ReadAllText(storePath);

            List<T> jsDes = JsonConvert.DeserializeObject<List<T>>(json);

            if (jsDes != null)
                return JsonConvert.DeserializeObject<List<T>>(json);
            else
                return new List<T>();
        }

    }
}

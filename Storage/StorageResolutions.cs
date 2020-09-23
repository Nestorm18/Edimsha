using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Edimsha.Edition.Editor;

namespace Edimsha.Storage
{
    class StorageResolutions : Storage
    {
        public StorageResolutions(string filePaths) : base(filePaths) { }

        internal List<Resolution> GetResolutions()
        {
            return GetObject<Resolution>();
        }

        internal bool SaveResolution(Resolution resolution)
        {
            var resolutions = GetObject<Resolution>();
            resolutions.Add(resolution);

            var temp = resolutions;

            // Remove Duplicates
            resolutions = resolutions.Distinct().ToList();

            File.WriteAllText(storePath, JsonConvert.SerializeObject(resolutions, Formatting.Indented));

            return !resolutions.SequenceEqual(temp);
        }

        internal void RemoveResolution(string res)
        {
            var splt = res.Split();

            var width = int.Parse(splt[1].Trim().Replace(",", ""));
            var height = int.Parse(splt[3].Trim());

            var resolutions = GetObject<Resolution>();

            // Remove Duplicates
            resolutions.RemoveAll(r => (r.Width == width && r.Height == height));

            File.WriteAllText(storePath, JsonConvert.SerializeObject(resolutions, Formatting.Indented));
        }
    }
}

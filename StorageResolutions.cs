using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

namespace Edimsha
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
            List<Resolution> resolutions = GetObject<Resolution>();
            resolutions.Add(resolution);

            List<Resolution> temp = resolutions;

            // Remove Duplicates
            resolutions = resolutions.Distinct().ToList();

            File.WriteAllText(storePath, JsonConvert.SerializeObject(resolutions, Formatting.Indented));

            return !resolutions.SequenceEqual(temp);
        }

        internal void RemoveResolution(string res)
        {
            string[] splt = res.Split();

            int width = int.Parse(splt[1].Trim().Replace(",", ""));
            int height = int.Parse(splt[3].Trim());

            List<Resolution> resolutions = GetObject<Resolution>();

            // Remove Duplicates
            resolutions.RemoveAll(r => (r.Width == width && r.Height == height));

            File.WriteAllText(storePath, JsonConvert.SerializeObject(resolutions, Formatting.Indented));
        }
    }
}

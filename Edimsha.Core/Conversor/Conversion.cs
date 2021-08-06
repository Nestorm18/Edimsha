using System;
using System.IO;
using Edimsha.Core.Models;

namespace Edimsha.Core.Conversor
{
    public class Conversion
    {
        private readonly string _path;
        private readonly ConversorConfig _config;

        public Conversion(string path, ConversorConfig config, ImageTypesConversor format)
        {
            _path = path;
            _config = config;
            
            FixNull();
        }
        
        private void FixNull()
        {
            _config.Edimsha ??= "edimsha_";

            if (_config.Edimsha.Equals(string.Empty))
                _config.Edimsha = "edimsha_";

            _config.OutputFolder ??= string.Empty;
        }

        public void Run()
        {
            var savePath = GeneratesavePath();

            

            Console.WriteLine(savePath);
        }
        
        private string GeneratesavePath()
        {
            var name = GenerateName();

            return _config.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(_path)?.FullName ?? string.Empty, name)
                : Path.Combine(_config.OutputFolder, name);
        }
        
        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileNameWithoutExtension(_path);

            if (_config.ReplaceForOriginal && !_config.AlwaysIncludeOnReplace)
                return imageName;

            if (samePath && !_config.AlwaysIncludeOnReplace) return imageName;

            var edimsha = _config.Edimsha;
            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath()
        {
            var outputDir = _config.OutputFolder;
            var currentDir = Directory.GetParent(_path)?.FullName;

            return outputDir == null || Equals(outputDir, currentDir);
        }
        
    }
}
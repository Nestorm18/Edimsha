// ReSharper disable UnusedMember.Global

namespace Edimsha.Core.Settings
{
    public class ConfigEditor
    {
        public string Language { get; set; }
        public bool CleanListOnExit { get; set; }
        public bool IterateSubdirectories { get; set; }
        public string OutputFolder { get; set; }
        public string Edimsha { get; set; }
        public bool AlwaysIncludeOnReplace { get; set; }
        public bool KeepOriginalResolution { get; set; }
        public double CompresionValue { get; set; }
        public bool OptimizeImage { get; set; }
        public bool ReplaceForOriginal { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
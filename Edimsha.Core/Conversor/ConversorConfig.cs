﻿namespace Edimsha.Core.Conversor
{
    public class ConversorConfig
    {
        public string Language { get; set; }
        public bool CleanListOnExit { get; set; }
        public bool IterateSubdirectories { get; set; }
        public string OutputFolder { get; set; }
        public string Edimsha { get; set; }
        public bool AlwaysIncludeOnReplace { get; set; }
        public bool ReplaceForOriginal { get; set; }
    }
}
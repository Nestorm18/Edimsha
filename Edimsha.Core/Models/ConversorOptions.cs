﻿using System.Collections.Generic;

namespace Edimsha.Core.Models
{
    public class ConversorOptions
    {
        public string Language { get; set; }
        public bool CleanListOnExit { get; set; }
        public bool IterateSubdirectories { get; set; }
        public int CurrentIndex { get; set; }
        public string OutputFolder { get; set; }
        public string Edimsha { get; set; }
        public List<string> Paths { get; set; }
    }
}
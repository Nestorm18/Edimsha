﻿using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace Edimsha.Core.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConversorOptions
    {
        public string Language { get; set; }
        public bool CleanListOnExit { get; set; }
        public bool IterateSubdirectories { get; set; }
        public int CurrentIndex { get; set; }
        public string OutputFolder { get; set; }
        public string Edimsha { get; set; }
        public List<string> Paths { get; set; }
        public ImageTypesConversor CurrentFormat { get; set; }
    }
}
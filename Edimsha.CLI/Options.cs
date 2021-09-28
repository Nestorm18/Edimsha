using System.Collections.Generic;
using CommandLine;
using Edimsha.Core.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Edimsha.CLI
{
    public class Options : IEditorOptionsCLI, IConversorOptionsCLI
    {
        [Option('m',
            "mode",
            Required = true,
            HelpText = "Set the run mode for the program. Valid values are \"editor\" and \"conversor\".")]
        public ViewType RunMode { get; set; }

        [Option('s',
            "subiter",
            HelpText = "Iterates the folders and subfolders that have been passed as paths.",
            Default = false)]
        public bool IterateSubdirectories { get; set; }

        [Option('f',
            "output",
            HelpText = "Save the images in the indicated folder.",
            Default = null)]
        public string OutputFolder { get; set; }

        [Option('e',
            "edimsha",
            HelpText = "If the image is saved in the same path, this prefix is added before.",
            Default = null)]
        public string Edimsha { get; set; }

        [Option('x',
            "paths",
            HelpText = "List of images to be processed separated by a spaces. (Always takes precedence over -p/--pathsasfolder)." +
                       "\nAvaliable format inputs:" +
                       "\n\t\t-> Editor: PNG, JPG." +
                       "\n\t\t-> Conversor: BMP, EMF, EXIF, GIF, ICO, JPG, PNG, TIFF, WMF.")]
        public IEnumerable<string> Paths { get; set; }

        [Option('p',
            "paths-as-folder",
            HelpText = "Path with the images to be processed.",
            Default = "")]
        public string PathsAsFolder { get; set; }

        public bool AlwaysIncludeOnReplace { get; set; }
        public bool KeepOriginalResolution { get; set; }
        public double CompresionValue { get; set; }
        public bool OptimizeImage { get; set; }
        public bool ReplaceForOriginal { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageTypesConversor ImageType { get; set; }
    }
}

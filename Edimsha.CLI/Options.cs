using CommandLine;
using Edimsha.Core.Models;

namespace Edimsha.CLI
{
    public class Options
    {
        [Option('m', "mode", Required = true, HelpText = "Set the run mode for the program. Valid values are Editor and Conversor.")]
        public ViewType RunMode { get; set; }

        [Option('s', "subiter", HelpText = "Iterates the folders and subfolders that have been passed as paths.", Default = false)]
        public bool IterateSubdirectories { get; set; }

        [Option('f', "output", HelpText = "Save the images in the indicated folder.", Default = null)]
        public string OutputFolder { get; set; }

        [Option('e', "edimsha", HelpText = "If the image is saved in the same path, this prefix is added before.", Default = null)]
        public string Edimsha { get; set; }

        [Option('i', "include", HelpText = "Include edimsha also in different folder.", Default = false)]
        public bool AlwaysIncludeOnReplace { get; set; }

        [Option('k', "keepresolution", HelpText = "Keeps the same resolution as the original image (can still optimize image).", Default = false)]
        public bool KeepOriginalResolution { get; set; }

        [Option('c', "compression", HelpText = "Adds the compression value of the image.", Default = 255)]
        public double CompresionValue { get; set; }

        [Option('o', "optimize", HelpText = "Optimize the image if it is png.", Default = false)]
        public bool OptimizeImage { get; set; }

        [Option('r', "replaze", HelpText = "Replaces the output image with the original one.", Default = false)]
        public bool ReplaceForOriginal { get; set; }

        [Option("resolution", HelpText = "The resolution at which to change the image.")]
        public Resolution Resolution { get; set; }

        [Option("paths", Required = true, HelpText = "List of images to be processed separated by a comma ','.", Separator = ',', Default = new string[0])]
        public string[] Paths { get; set; }

        [Option("pathsasfolder", Required = true, HelpText = "Path with the images to be processed.", Default = "")]
        public string PathsAsFolder { get; set; }
    }
}
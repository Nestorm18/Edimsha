using CommandLine;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable InconsistentNaming

namespace Edimsha.CLI
{
    public interface IEditorOptionsCLI
    {
        [Option('i', "include",
            SetName = "Editor",
            HelpText = "Include edimsha also in different folder.",
            Default = false)]
        public bool AlwaysIncludeOnReplace { get; set; }

        [Option('k', "keep-resolution",
            SetName = "Editor",
            HelpText = "Keeps the same resolution as the original image (can still optimize image).",
            Default = false)]
        public bool KeepOriginalResolution { get; set; }

        [Option('c', "compression",
            SetName = "Editor"
            , HelpText = "Adds the compression value of the image.",
            Default = 255)]
        public double CompresionValue { get; set; }

        [Option('o', "optimize",
            SetName = "Editor",
            HelpText = "Optimize the image if it is png.",
            Default = false)]
        public bool OptimizeImage { get; set; }

        [Option('r', "replaze",
            SetName = "Editor",
            HelpText = "Replaces the output image with the original one.",
            Default = false)]
        public bool ReplaceForOriginal { get; set; }

        [Option('w', "width",
            SetName = "Editor",
            Required = true,
            HelpText = "The resolution at which to change the image.",
            Default = -1)]
        public int Width { get; set; }

        [Option('h', "height",
            SetName = "Editor",
            Required = true,
            HelpText = "The resolution at which to change the image.",
            Default = -1)]
        public int Height { get; set; }
    }
}
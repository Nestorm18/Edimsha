using CommandLine;
using Edimsha.Core.Models;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable InconsistentNaming

namespace Edimsha.CLI
{
    public interface IConversorOptionsCLI
    {
        [Option('t', "format",Required = true,
            SetName = "Conversor",
            HelpText = "The format to convert the image to.\nAvaliable: BMP, EMF, EXIF, GIF, ICO, JPG, PNG, TIFF, WMF.",
            Default = ImageTypesConversor.PNG)]
        public ImageTypesConversor ImageType { get; set; }
    }
}
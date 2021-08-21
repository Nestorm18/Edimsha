// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Edimsha.Core.Models
{
    /// <summary>
    /// Modes in app.
    /// </summary>
    public enum ViewType
    {
        Editor,
        Converter
    }
    
    /// <summary>
    /// Image formats available for editor mode..
    /// </summary>
    public enum ImageTypesEditor
    {
        PNG,
        JPG,
        JPEG
    }

    /// <summary>
    /// Image formats available for conversor mode.
    /// </summary>
    public enum ImageTypesConversor
    {
        BMP,
        EMF,
        EXIF,
        GIF,
        ICO,
        JPG,
        PNG,
        TIFF,
        WMF
    }
}
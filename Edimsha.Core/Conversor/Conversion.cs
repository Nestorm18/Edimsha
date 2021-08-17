using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Edimsha.Core.Models;

namespace Edimsha.Core.Conversor
{
    public class Conversion
    {
        private readonly string _path;
        private readonly ConversorOptions _options;
        private readonly ImageTypesConversor _format;

        public Conversion(string path, ConversorOptions options, ImageTypesConversor format)
        {
            _path = path;
            _options = options;
            _format = format;

            FixNull();
        }

        private void FixNull()
        {
            _options.Edimsha ??= "edimsha_";

            if (_options.Edimsha.Equals(string.Empty))
                _options.Edimsha = "edimsha_";

            _options.OutputFolder ??= string.Empty;
        }

        public void Run()
        {
            // Avoid converting to the same image format
            if (IsSameFormatCurrentImage()) return;
            
            var savePath = GeneratesavePath();

            savePath += $".{_format.ToString().ToLower()}";

            var imageFormat = ImageTypesConversorToImageFormat();

            using var img = Image.FromFile(_path);

            img.Save(savePath, imageFormat);

            img.Dispose();
        }

        private bool IsSameFormatCurrentImage()
        {
            var extension = Path.GetExtension(_path);

            return extension != null && extension.Equals($".{_format.ToString()}");
        }

        private ImageFormat ImageTypesConversorToImageFormat()
        {
            return _format switch
            {
                ImageTypesConversor.BMP => ImageFormat.Bmp,
                ImageTypesConversor.EMF => ImageFormat.Emf,
                ImageTypesConversor.EXIF => ImageFormat.Exif,
                ImageTypesConversor.GIF => ImageFormat.Gif,
                ImageTypesConversor.ICO => ImageFormat.Icon,
                ImageTypesConversor.JPG => ImageFormat.Jpeg,
                ImageTypesConversor.PNG => ImageFormat.Png,
                ImageTypesConversor.TIFF => ImageFormat.Tiff,
                ImageTypesConversor.WMF => ImageFormat.Wmf,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GeneratesavePath()
        {
            var name = GenerateName();

            return _options.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(_path)?.FullName ?? string.Empty, name)
                : Path.Combine(_options.OutputFolder, name);
        }

        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileNameWithoutExtension(_path);
            var edimsha = _options.Edimsha;

            if (edimsha.Equals("edimsha_") && samePath) return imageName;

            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath()
        {
            var outputDir = _options.OutputFolder;
            var currentDir = Directory.GetParent(_path)?.FullName;

            return string.IsNullOrEmpty(outputDir) || Equals(outputDir, currentDir);
        }
    }
}
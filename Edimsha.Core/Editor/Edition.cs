using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;

namespace Edimsha.Core.Editor
{
    public class Edition
    {
        private readonly string _path;
        private readonly ConfigEditor _configEditor;
        public Resolution Resolution { get; set; }

        public Edition(string path, ConfigEditor configEditor)
        {
            _path = path;
            _configEditor = configEditor;

            FixNull();
        }

        private void FixNull()
        {
            _configEditor.Edimsha ??= "edimsha_";

            if (_configEditor.Edimsha.Equals(string.Empty))
                _configEditor.Edimsha = "edimsha_";

            _configEditor.OutputFolder ??= string.Empty;
        }

        public void Run()
        {
            // Image resize to user values
            Image image;

            using (var img = Image.FromFile(_path))
            {
                if ((bool) _configEditor.KeepOriginalResolution || (Resolution.Width <= 0 || Resolution.Height <= 0))
                    Resolution = new Resolution(img.Width, img.Height);

                image = Resize(img);
            }

            var savePath = GeneratesavePath();

            if ((bool) _configEditor.AlwaysIncludeOnReplace)
                File.Delete(_path);

            if ((bool) _configEditor.OptimizeImage)
                image.Save(savePath, ImageFormat.Jpeg);
            else
                image.Save(savePath, ImageFormat.Jpeg);

            image.Dispose();
        }

        private string GeneratesavePath()
        {
            var name = GenerateName();

            return _configEditor.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(_path)?.FullName ?? string.Empty, name)
                : Path.Combine((string) _configEditor.OutputFolder, name);
        }

        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileName(_path);

            if ((bool) _configEditor.ReplaceForOriginal && !(bool) _configEditor.AlwaysIncludeOnReplace)
                return imageName;

            if (samePath && !(bool) _configEditor.AlwaysIncludeOnReplace) return imageName;

            var edimsha = _configEditor.Edimsha;
            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath()
        {
            var outputDir = _configEditor.OutputFolder;
            var currentDir = Directory.GetParent(_path)?.FullName;

            return outputDir == null || Equals(outputDir, currentDir);
        }

        private Image Resize(Image image)
        {
            return FixedSize(image, Resolution.Width, Resolution.Height);
        }

        private static Image FixedSize(Image imgPhoto, int width, int height)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            var destX = 0;
            var destY = 0;

            float nPercent;
            var nPercentW = width / (float) sourceWidth;
            var nPercentH = height / (float) sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width - sourceWidth * nPercent) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height - sourceHeight * nPercent) / 2);
            }

            var destWidth = (int) (sourceWidth * nPercent);
            var destHeight = (int) (sourceHeight * nPercent);

            var bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            var grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White); // Backgroundcolor!
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
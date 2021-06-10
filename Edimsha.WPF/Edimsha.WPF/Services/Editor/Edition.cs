using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Edimsha.WPF.Models;
using Edimsha.WPF.Settings;

namespace Edimsha.WPF.Services.Editor
{
    public class Edition
    {
        private readonly string _path;
        private readonly Config _config;
        public Resolution Resolution { get; set; }

        public Edition(string path, Config config)
        {
            _path = path;
            _config = config;

            FixNull();
        }

        private void FixNull()
        {
            _config.Edimsha ??= "edimsha_";
            _config.OutputFolder ??= string.Empty;
        }

        public void Run()
        {
            // Image resize to user values
            Image image;

            using (var img = Image.FromFile(_path))
            {
                if ((bool) _config.KeepOriginalResolution || (Resolution.Width <= 0 || Resolution.Height <= 0))
                    Resolution = new Resolution {Width = img.Width, Height = img.Height};

                image = Resize(img);
            }
            
            var savePath = GeneratesavePath();
            
            if ((bool) _config.AlwaysIncludeOnReplace)
                File.Delete(_path);
            
            if ((bool) _config.OptimizeImage)
                image.Save(savePath, ImageFormat.Jpeg);
            else
                image.Save(savePath, ImageFormat.Jpeg);

            image.Dispose();
        }

        private string GeneratesavePath()
        {
            var name = GenerateName();

            return _config.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(_path)?.FullName ?? string.Empty, name)
                : Path.Combine((string) _config.OutputFolder, name);
        }

        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileName(_path);
            
            if ((bool) _config.ReplaceForOriginal && !(bool) _config.AlwaysIncludeOnReplace)
                return imageName;

            if (samePath && !(bool) _config.AlwaysIncludeOnReplace) return imageName;
            
            var edimsha =  _config.Edimsha;
            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath()
        {
            var outputDir = _config.OutputFolder;
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
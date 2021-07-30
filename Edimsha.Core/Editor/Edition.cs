using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Edimsha.Core.Models;

namespace Edimsha.Core.Editor
{
    public class Edition
    {
        private readonly string _path;
        private readonly EditorConfig _editorConfig;

        public Edition(string path, EditorConfig editorConfig)
        {
            _path = path;
            _editorConfig = editorConfig;

            FixNull();
        }

        private void FixNull()
        {
            _editorConfig.Edimsha ??= "edimsha_";

            if (_editorConfig.Edimsha.Equals(string.Empty))
                _editorConfig.Edimsha = "edimsha_";

            _editorConfig.OutputFolder ??= string.Empty;
        }

        public void Run()
        {
            // Image resize to user values
            Image image;

            using (var img = Image.FromFile(_path))
            {
                if (_editorConfig.KeepOriginalResolution || _editorConfig.Resolution.Width <= 0 || _editorConfig.Resolution.Height <= 0)
                    _editorConfig.Resolution = new Resolution(img.Width, img.Height);

                image = Resize(img);
            }

            var savePath = GeneratesavePath();

            if (_editorConfig.AlwaysIncludeOnReplace)
                File.Delete(_path);

            var extension = Path.GetExtension(_path);

            // ReSharper disable once PossibleNullReferenceException
            if (extension.Equals(".jpeg") || extension.Equals(".jpg"))
            {
                image.Save(string.Concat(savePath, extension));
            }
            else
            {
                var pathWithExtension = string.Concat(savePath, ".png");

                if (_editorConfig.OptimizeImage)
                {
                    image.Save(pathWithExtension, ImageFormat.Png);

                    PngQuant.ExecutePngQuant(pathWithExtension, _editorConfig.CompresionValue);
                }
                else
                {
                    image.Save(pathWithExtension, ImageFormat.Png);
                }
            }

            image.Dispose();
        }

        private string GeneratesavePath()
        {
            var name = GenerateName();

            return _editorConfig.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(_path)?.FullName ?? string.Empty, name)
                : Path.Combine(_editorConfig.OutputFolder, name);
        }

        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileNameWithoutExtension(_path);

            if (_editorConfig.ReplaceForOriginal && !_editorConfig.AlwaysIncludeOnReplace)
                return imageName;

            if (samePath && !_editorConfig.AlwaysIncludeOnReplace) return imageName;

            var edimsha = _editorConfig.Edimsha;
            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath()
        {
            var outputDir = _editorConfig.OutputFolder;
            var currentDir = Directory.GetParent(_path)?.FullName;

            return outputDir == null || Equals(outputDir, currentDir);
        }

        private Image Resize(Image image)
        {
            return FixedSize(image, _editorConfig.Resolution.Width, _editorConfig.Resolution.Height);
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
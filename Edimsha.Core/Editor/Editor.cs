using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Edimsha.Core.Models;

namespace Edimsha.Core.Editor
{
    public class Editor
    {
        private readonly EditorOptions _options;

        public Editor(EditorOptions options)
        {
            _options = options;
        }

        public async Task ExecuteProcessing(IProgress<ProgressReport> progress, CancellationTokenSource cancellationToken)
        {
            FixNull();
            
            var imageIndex = 1;
            var pathCount = _options.Paths.Count;
            
            foreach (var path in _options.Paths)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    progress.Report(new ProgressReport {ReportType = ReportType.MessageA, Data = "cancelled_by_user"});
                    progress.Report(new ProgressReport {ReportType = ReportType.MessageB, Data = ""});
                    break;
                }
                
                progress.Report(new ProgressReport {ReportType = ReportType.Percent, Data = imageIndex * 100 / pathCount});
                progress.Report(new ProgressReport {ReportType = ReportType.MessageA, Data = "procesing"});
                progress.Report(new ProgressReport {ReportType = ReportType.MessageB, Data = $"{imageIndex} -> {pathCount}"});
                
                // Image resize to user values
                Image image;
                
                using (var img = Image.FromFile(path))
                {
                    var width = _options.Resolution.Width;
                    var height = _options.Resolution.Height;

                    if (_options.KeepOriginalResolution || width <= 0 || height <= 0)
                        _options.Resolution = new Resolution(img.Width, img.Height);

                    image = FixedSize(img, width, height);
                }
                
                var savePath = GeneratesavePath(path);

                if (_options.AlwaysIncludeOnReplace)
                    File.Delete(path);

                var extension = Path.GetExtension(path);

                // ReSharper disable once PossibleNullReferenceException
                if (extension.Equals(".jpeg") || extension.Equals(".jpg")) 
                    image.Save(string.Concat(savePath, extension));
                else
                    SaveAsPng(savePath, image);

                image.Dispose();

                imageIndex++;
            }
            
            progress.Report(new ProgressReport {ReportType = ReportType.Finalizated, Data = true});
            
        }

        private void FixNull()
        {
            _options.Edimsha ??= "edimsha_";

            if (_options.Edimsha.Equals(string.Empty))
                _options.Edimsha = "edimsha_";

            _options.OutputFolder ??= string.Empty;
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

        private string GeneratesavePath(string path)
        {
            var name = GenerateName(path);

            return _options.OutputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(path)?.FullName ?? string.Empty, name)
                : Path.Combine(_options.OutputFolder, name);
        }

        private string GenerateName(string path)
        {
            var samePath = IsSamePath(path);
            var imageName = Path.GetFileNameWithoutExtension(path);

            if (_options.ReplaceForOriginal && !_options.AlwaysIncludeOnReplace)
                return imageName;

            if (samePath && !_options.AlwaysIncludeOnReplace) return imageName;

            var edimsha = _options.Edimsha;
            return $"{edimsha}{imageName}";
        }

        private bool IsSamePath(string path)
        {
            var outputDir = _options.OutputFolder;
            var currentDir = Directory.GetParent(path)?.FullName;

            return outputDir == null || Equals(outputDir, currentDir);
        }

        private void SaveAsPng(string savePath, Image image)
        {
            var pathWithExtension = string.Concat(savePath, ".png");

            if (_options.OptimizeImage)
            {
                image.Save(pathWithExtension, ImageFormat.Png);

                PngQuant.ExecutePngQuant(pathWithExtension, _options.CompresionValue);
            }
            else
            {
                image.Save(pathWithExtension, ImageFormat.Png);
            }
        }
    }
}
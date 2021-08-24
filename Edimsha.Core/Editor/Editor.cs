using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Edimsha.Core.BaseImageEdition;
using Edimsha.Core.Models;

namespace Edimsha.Core.Editor
{
    public class Editor : BaseEditor
    {
        private readonly EditorOptions _options;

        /// <summary>
        /// It is used to convert a series of options in which there are image paths and resolutions to convert
        /// to when the <see cref="ExecuteProcessing"/> method is executed.
        /// </summary>
        /// <param name="options">Options to use when processing images.</param>
        public Editor(EditorOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Runs a task that resize the images passed as a list in the <see cref="EditorOptions"/> class to the new size.
        /// All options used in this method come from <see cref="EditorOptions"/>.  
        /// </summary>
        /// <param name="progress">Used to report progress on each iteration or warn of errors.</param>
        /// <param name="cancellationToken">Stop the execution of image processing as soon as possible.</param>
        /// <returns></returns>
        public async Task ExecuteProcessing(IProgress<ProgressReport> progress, CancellationTokenSource cancellationToken)
        {
            _options.Edimsha ??= "edimsha_";

            if (_options.Edimsha .Equals(string.Empty))
                _options.Edimsha  = "edimsha_";

            _options.OutputFolder  ??= string.Empty;
            
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
                
                var savePath = GeneratesavePath(_options.OutputFolder, path, _options.Edimsha);

                if (_options.ReplaceForOriginal)
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

        /// <summary>
        /// Resize an image with the values provided as parameters.
        /// </summary>
        /// <param name="imgPhoto">The image to be redimensioned.</param>
        /// <param name="width">The new image width.</param>
        /// <param name="height">The new image height.</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Used to perform an optimization in case it is indicated in the options using <see cref="PngQuant"/>.
        /// </summary>
        /// <param name="savePath">The path where the image will be saved.</param>
        /// <param name="image">The image to save.</param>
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
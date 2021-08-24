using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Edimsha.Core.BaseImageEdition;
using Edimsha.Core.Models;

namespace Edimsha.Core.Conversor
{
    public class Conversor : BaseEditor
    {
        private readonly ConversorOptions _options;

        /// <summary>
        /// It is used to convert a series of options in which there are image paths and formats to convert
        /// to when the <see cref="ExecuteProcessing"/> method is executed.
        /// </summary>
        /// <param name="options">Options to use when processing images.</param>
        public Conversor(ConversorOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Runs a task that converts the images passed as a list in the <see cref="ConversorOptions"/> class to the new format.
        /// All options used in this method come from <see cref="ConversorOptions"/>.  
        /// </summary>
        /// <param name="progress">Used to report progress on each iteration or warn of errors.</param>
        /// <param name="cancellationToken">Stop the execution of image processing as soon as possible.</param>
        /// <returns></returns>
        public async Task ExecuteProcessing(IProgress<ProgressReport> progress, CancellationTokenSource cancellationToken)
        {
            _options.Edimsha ??= "edimsha_";

            if (_options.Edimsha.Equals(string.Empty))
                _options.Edimsha = "edimsha_";

            _options.OutputFolder ??= string.Empty;

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

                // Avoid converting to the same image format
                if (IsSameFormatCurrentImage(path)) return;

                var savePath = GeneratesavePath(_options.OutputFolder, path, _options.Edimsha, false);

                savePath += $".{_options.CurrentFormat.ToString()}";

                var imageFormat = ImageTypesConversorToImageFormat();

                using var img = Image.FromFile(path);

                img.Save(savePath, imageFormat);

                img.Dispose();

                imageIndex++;
            }

            progress.Report(new ProgressReport {ReportType = ReportType.Finalizated, Data = true});
        }

        /// <summary>
        /// Check if the image is already in the format to be converted.
        /// </summary>
        /// <param name="path">A current image path.</param>
        /// <returns>True if has same format.</returns>
        private bool IsSameFormatCurrentImage(string path)
        {
            var extension = Path.GetExtension(path);

            return extension != null && extension.Equals($".{_options.CurrentFormat.ToString()}");
        }

        /// <summary>
        /// Parse an enumeration of type ImageTypesConverter to ImageFormat.
        /// </summary>
        /// <returns>The corresponding value for ImageFormat.</returns>
        private ImageFormat ImageTypesConversorToImageFormat()
        {
            return _options.CurrentFormat switch
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
    }
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Edimsha.Properties;

namespace Edimsha.Edition.Editor
{
    internal class Edition
    {
        public Edition(string path)
        {
            ImagePath = path;
        }

        public string ImagePath { get; set; }

        public int Width { get; internal set; }

        public int Height { get; internal set; }

        public string OutputFolder { get; internal set; }

        public bool ReplaceOriginal { get; internal set; }

        public bool AddOnReplace { get; internal set; }

        public string Edimsha { get; internal set; }

        public bool OriginalResolution { get; internal set; }

        public bool OptimizeImage { get; internal set; }

        public int CompressionValue { get; internal set; }

        internal void Run()
        {
            // Image resize to user values
            Image image;

            using (var img = Image.FromFile(ImagePath))
            {
                if (OriginalResolution)
                {
                    Width = img.Width;
                    Height = img.Height;
                }

                image = Resize(img);
            }

            var savePath = GeneratesavePath();

            if (AddOnReplace)
                File.Delete(ImagePath);

            if (Settings.Default.chkOptimizeImage)
            {
                Console.WriteLine("Runnning quant");
                image.Save(savePath, ImageFormat.Jpeg);
            }
            else
            {
                image.Save(savePath, ImageFormat.Jpeg);
            }

            image.Dispose();
        }

        private string GeneratesavePath()
        {
            var name = GenerateName();

            if (Settings.Default.txtEditorFolderPath.Equals(""))
                return Path.Combine(Directory.GetParent(ImagePath).FullName, name);

            return Path.Combine(OutputFolder, name);
        }

        private string GenerateName()
        {
            var samePath = IsSamePath();
            var imageName = Path.GetFileName(ImagePath);

            if (ReplaceOriginal && !AddOnReplace)
                return imageName;

            if (samePath || AddOnReplace && !samePath)
            {
                var edimsha = Edimsha;
                return $"{edimsha}{imageName}";
            }

            return imageName;
        }

        private bool IsSamePath()
        {
            var outputDir = OutputFolder;
            var currentDir = Directory.GetParent(ImagePath).FullName;

            if (outputDir.Equals(""))
                return true;

            return Equals(outputDir, currentDir);
        }

        private Image Resize(Image image)
        {
            return FixedSize(image, Width, Height);
        }

        private static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;

            var nPercent = 0.0f;
            var nPercentW = Width / (float) sourceWidth;
            var nPercentH = Height / (float) sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((Width - sourceWidth * nPercent) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((Height - sourceHeight * nPercent) / 2);
            }

            var destWidth = (int) (sourceWidth * nPercent);
            var destHeight = (int) (sourceHeight * nPercent);

            var bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
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
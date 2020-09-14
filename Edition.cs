using Edimsha.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Edimsha
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

            string savePath = GeneratesavePath();

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
            string name = GenerateName();

            if (Settings.Default.txtEditorFolderPath.Equals(""))
                return Path.Combine(Directory.GetParent(ImagePath).FullName, name);

            return Path.Combine(OutputFolder, name);
        }

        private string GenerateName()
        {
            bool samePath = IsSamePath();
            string imageName = Path.GetFileName(ImagePath);

            bool replaceOriginal = ReplaceOriginal;
            bool replaceEdimsha = AddOnReplace;

            if (replaceOriginal && !replaceEdimsha)
                return imageName;

            if (samePath || (replaceEdimsha && !samePath))
            {
                string edimsha = Edimsha;
                return $"{edimsha}{imageName}";
            }

            return imageName;
        }

        private bool IsSamePath()
        {
            string outputDir = OutputFolder;
            string currentDir = Directory.GetParent(ImagePath).FullName;

            if (outputDir.Equals(""))
                return true;

            return Equals(outputDir, currentDir);
        }

        private Image Resize(Image image) => FixedSize(image, Width, Height);

        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent;
            float nPercentW;
            float nPercentH;

            nPercentW = Width / (float)sourceWidth;
            nPercentH = Height / (float)sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
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
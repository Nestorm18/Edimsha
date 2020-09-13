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
        private readonly int width;
        private readonly int height;

        public Edition(string path)
        {
            ImagePath = path;
            width = int.Parse(Settings.Default.Width);
            height = int.Parse(Settings.Default.Height);
        }

        public string ImagePath { get; set; }

        internal void Run()
        {
            // Image resize to user values
            Image image = Resize(Image.FromFile(ImagePath));
            string savePath = GeneratesavePath();

            Console.WriteLine(savePath);

            if (Settings.Default.chkOptimizeImage)
            {
                Console.WriteLine("Runnning quant");
                image.Save(savePath, ImageFormat.Jpeg);
            }
            else
            {
                image.Save(savePath, ImageFormat.Jpeg);
            }
        }

        private string GeneratesavePath()
        {
            string name = GenerateName();
                        
            if (Settings.Default.txtEditorFolderPath.Equals(""))            
                return Path.Combine(Directory.GetParent(ImagePath).FullName, name);
            
            return Path.Combine(Settings.Default.txtEditorFolderPath, name);
        }

        private string GenerateName()
        {
            bool samePath = IsSamePath();
            string imageName = Path.GetFileName(ImagePath);

            bool replaceOriginal = Settings.Default.chkReplaceForOriginal;
            bool replaceEdimsha = Settings.Default.chkAddOnReplace;


            string edimsha = Settings.Default.txtEdimsha;

            if (replaceOriginal && !replaceEdimsha)            
                edimsha = "";

            if (samePath || (replaceEdimsha && !samePath))
            {
                edimsha = Settings.Default.txtEdimsha;
                return $"{edimsha}{imageName}";
            }                     

            return imageName;
        }

        private bool IsSamePath()
        {
            string outputDir = Settings.Default.txtEditorFolderPath;
            string currentDir = Directory.GetParent(ImagePath).FullName;

            if (outputDir.Equals(""))            
                return true;           

            return Equals(outputDir, currentDir);
        }

        private Image Resize(Image image) => FixedSize(image, width, height);

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
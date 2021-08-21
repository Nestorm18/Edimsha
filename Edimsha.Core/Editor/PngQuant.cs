using System;
using System.Diagnostics;
using System.IO;

namespace Edimsha.Core.Editor
{
    public static class PngQuant
    {
        /// <summary>
        /// The path where the pngquant executable is found.
        /// </summary>
        private static readonly string Pngquant = Path.Combine(Environment.CurrentDirectory, "Resources", "pngquant.exe");

        /// <summary>
        /// Run the pngquant executable to optimize the image using the specified parameters.
        /// <para>Basically it reduces the bit number from the maximum of 255 to a lower value,
        /// with each lower value the image will reduce in size in storage space.</para>
        /// </summary>
        /// <param name="path">The image path to optimize.</param>
        /// <param name="bitNumber">The number of bits that will form the image after processing.</param>
        public static void ExecutePngQuant(string path, double bitNumber)
        {
            if (bitNumber > 255) bitNumber = 255;

            var psi = new ProcessStartInfo(Pngquant)
            {
                Arguments = $"-f --ext {Path.GetExtension(path)} --skip-if-larger --quality {(int) bitNumber} \"{path}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }
    }
}
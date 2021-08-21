using System;
using System.Diagnostics;
using System.IO;

namespace Edimsha.Core.Editor
{
    public static class PngQuant
    {
        private static readonly string Pngquant = Path.Combine(Environment.CurrentDirectory, "Resources", "pngquant.exe");

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
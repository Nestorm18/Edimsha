using Edimsha.Properties;
using System;

namespace Edimsha
{
    internal class Edition
    {
        private int width;
        private int height;

        public Edition(string path)
        {
            Path = path;
            width = int.Parse(Settings.Default.Width);
            height = int.Parse(Settings.Default.Height);
        }

        public string Path { get; set; }

        internal void Run()
        {
            throw new NotImplementedException();
        }
    }
}
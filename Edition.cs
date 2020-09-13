using System;

namespace Edimsha
{
    internal class Edition
    {
        public string Path { get; set; }

        public Edition(string path)
        {
            Path = path;
        }

        internal void Run()
        {
            throw new NotImplementedException();
        }
    }
}
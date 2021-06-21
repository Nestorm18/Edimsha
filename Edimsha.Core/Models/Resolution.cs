#nullable enable
using System;

namespace Edimsha.Core.Models
{
    public class Resolution
    {
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public override string ToString()
        {
            return $"X: {Width}, Y: {Height}";
        }

        public bool Equals(Resolution other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public bool IsValid()
        {
            Console.WriteLine(Width > 0 && Height > 0);
            return Width > 0 && Height > 0;
        }
    }
}
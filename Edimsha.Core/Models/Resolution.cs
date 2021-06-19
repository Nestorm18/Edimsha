#nullable enable
using System;

namespace Edimsha.Core.Models
{
    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

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
#nullable enable
using System;

namespace Edimsha.WPF.Models
{
    public class Resolution
    {
        public int Width { get; init; }
        public int Height { get; init; }

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
    }
}
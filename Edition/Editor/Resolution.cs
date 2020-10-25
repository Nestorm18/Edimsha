﻿using System;

namespace Edimsha.Edition.Editor
{
    internal class Resolution : IEquatable<Resolution>
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public bool Equals(Resolution other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }
    }
}
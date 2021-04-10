namespace Edimsha.WPF.Models
{
    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return $"X: {Width}, Y: {Height}";
        }
    }
}
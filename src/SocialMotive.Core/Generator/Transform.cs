namespace SocialMotive.Core.Data.Generator
{
    public class Transform
    {
        // Position
        public decimal? PositionX { get; set; }
        public decimal? PositionY { get; set; }

        // Size
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        // Rotation
        public decimal? Rotation { get; set; } = 0;

        // Zoom (1 = 100%, 0.5 = 50%, 2 = 200%)
        public decimal? Zoom { get; set; } = 1;
    }
}

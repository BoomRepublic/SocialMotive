namespace SocialMotive.Core.Data.Generator
{
    public class LayerSettings
    {
        // Transform
        public Transform? Transform { get; set; }

        // Opacity
        public decimal Opacity { get; set; } = 1;

        // Visibility properties
        public bool IsVisible { get; set; } = true;
        public bool IsLocked { get; set; } = false;

        // Style properties
        public string? BackgroundColor { get; set; }
        public string? BlendMode { get; set; } // "normal", "multiply", "screen", "overlay", etc.
    }
}

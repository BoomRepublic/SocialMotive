namespace SocialMotive.Core.Data.Generator
{
    public class ShapeLayerSettings : LayerSettings
    {
        // Shape type
        public string ShapeType { get; set; } = "rectangle"; // "rectangle", "circle", "ellipse", "polygon", "star", "line", "path"

        // Fill properties
        public string? FillColor { get; set; } = "#000000";
        public decimal? FillOpacity { get; set; }

        // Stroke/Border (different from LayerSettings.BorderColor for more control)
        public string? StrokeColor { get; set; } = "#000000";
        public decimal? StrokeWidth { get; set; }
        public string? StrokeDasharray { get; set; } // e.g., "5,5" for dashed lines
        public string? StrokeLinecap { get; set; } // "butt", "round", "square"
        public string? StrokeLinejoin { get; set; } // "miter", "round", "bevel"

        // Rounded corners (for rectangles)
        public decimal? BorderRadiusTL { get; set; } // Top-left
        public decimal? BorderRadiusTR { get; set; } // Top-right
        public decimal? BorderRadiusBR { get; set; } // Bottom-right
        public decimal? BorderRadiusBL { get; set; } // Bottom-left

        // Polygon specific
        public int? PolygonSides { get; set; } // Number of sides for regular polygons

        // Star specific
        public int? StarPoints { get; set; } // Number of points
        public decimal? StarInnerRadius { get; set; } // Inner radius ratio

        // Gradient/Pattern (stored as JSON string)
        public string? GradientConfig { get; set; } // JSON: { type: "linear|radial", angle, stops: [{offset, color}, ...] }

        // Shadow
        public string? ShadowColor { get; set; }
        public decimal? ShadowOffsetX { get; set; }
        public decimal? ShadowOffsetY { get; set; }
        public decimal? ShadowBlur { get; set; }
    }
}

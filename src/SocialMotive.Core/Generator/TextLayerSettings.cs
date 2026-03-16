namespace SocialMotive.Core.Data.Generator
{
    public class TextLayerSettings : LayerSettings
    {
        // Text content
        public string? TextContent { get; set; }

        // Text spacing and sizing
        public decimal? LineHeight { get; set; }
        public decimal? LetterSpacing { get; set; }

        // Font styling
        public string? FontWeight { get; set; } // "normal", "bold", "100-900"
        public string? FontStyle { get; set; } // "normal", "italic", "oblique"
        public string? TextDecoration { get; set; } // "none", "underline", "overline", "line-through"

        // Text transformation
        public string? TextTransform { get; set; } // "none", "uppercase", "lowercase", "capitalize"

        // Text effects
        public string? TextShadow { get; set; } // e.g., "2px 2px 4px rgba(0,0,0,0.5)"

        // Text layout
        public bool WordWrap { get; set; } = true;
        public int? MaxLines { get; set; }
        public string? TextOverflow { get; set; } // "clip", "ellipsis"

        // Text direction
        public string? Direction { get; set; } // "ltr", "rtl"
    }
}

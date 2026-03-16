namespace SocialMotive.Core.Data.Generator
{
    public class ImageLayerSettings : LayerSettings
    {
        // Image source
        public string? ImageUrl { get; set; }
        public int? AssetId { get; set; }

        // Scaling and fit
        public string? ObjectFit { get; set; } // "fill", "contain", "cover", "scale-down"
        public string? ObjectPosition { get; set; } // e.g., "center", "top left", "50% 50%"

        // Image filters
        public decimal? FilterBrightness { get; set; } // 0-200 (100 = normal)
        public decimal? FilterContrast { get; set; } // 0-200 (100 = normal)
        public decimal? FilterSaturation { get; set; } // 0-200 (100 = normal)
        public decimal? FilterHue { get; set; } // 0-360 degrees
        public decimal? FilterInvert { get; set; } // 0-100 (percentage)
        public decimal? FilterGrayscale { get; set; } // 0-100 (percentage)
        public decimal? FilterSepia { get; set; } // 0-100 (percentage)
        public decimal? FilterBlur { get; set; } // pixels

        // Image effects
        public string? ShadowColor { get; set; }
        public decimal? ShadowOffsetX { get; set; }
        public decimal? ShadowOffsetY { get; set; }
        public decimal? ShadowBlur { get; set; }

        // Border/Frame
        public string? BorderColor { get; set; }
        public decimal? BorderWidth { get; set; }
        public decimal? BorderRadius { get; set; } // rounded corners

        // Clipping/Mask
        public string? ClipShape { get; set; } // "circle", "rectangle", "polygon", etc.
        public string? MaskJson { get; set; } // SVG mask definition as JSON
    }
}

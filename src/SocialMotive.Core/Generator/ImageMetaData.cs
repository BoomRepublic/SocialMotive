namespace SocialMotive.Core.Data.Generator
{
    public class ImageMetaData
    {
        // Dimensions
        public int? Width { get; set; }
        public int? Height { get; set; }

        // File info
        public string? MimeType { get; set; }
        public long? FileSize { get; set; } // bytes
        public int? DpiX { get; set; }
        public int? DpiY { get; set; }

        // Color info
        public int? BitDepth { get; set; }
        public bool? HasAlpha { get; set; }
        public string? ColorSpace { get; set; } // "sRGB", "AdobeRGB", etc.
    }
}

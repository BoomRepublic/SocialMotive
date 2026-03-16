namespace SocialMotive.Core.Model.Generator
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ImageMetaDataJson { get; set; }
        public string? Tags { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

namespace SocialMotive.Core.Data.Generator
{
    public class DbAsset
    {
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[]? ImagePng { get; set; }
        public string? ImageMetaDataJson { get; set; }
        public string? Tags { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
    }
}

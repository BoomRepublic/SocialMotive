namespace SocialMotive.Core.Data.Generator
{
    public class DbLayer
    {
        public int LayerId { get; set; }
        public int TemplateId { get; set; }
        public string LayerType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ZIndex { get; set; }
        public int? AssetId { get; set; }
        public string? SettingsJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public DbTemplate? Template { get; set; }
        public DbAsset? Asset { get; set; }
    }
}

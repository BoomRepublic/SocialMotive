namespace SocialMotive.Core.Model.Generator
{
    public class Layer
    {
        public int LayerId { get; set; }
        public string LayerType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ZIndex { get; set; }
        public int? AssetId { get; set; }
        public string? SettingsJson { get; set; }
    }
}

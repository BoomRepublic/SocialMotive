namespace SocialMotive.Core.Model.Generator
{
    public class TemplateDetail
    {
        public int TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPublished { get; set; }
        public bool IsTemplate { get; set; }
        public string? Tags { get; set; }
        public string? Category { get; set; }
        public List<Layer> Layers { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

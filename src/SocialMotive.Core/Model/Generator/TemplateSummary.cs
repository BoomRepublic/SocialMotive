namespace SocialMotive.Core.Model.Generator
{
    public class TemplateSummary
    {
        public int TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPublished { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

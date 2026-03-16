namespace SocialMotive.Core.Model.Generator
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Tags { get; set; }
        public string? Category { get; set; }
    }
}

namespace SocialMotive.Core.Model.Generator
{
    public class UpdateTemplateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool? IsPublished { get; set; }
        public string? Tags { get; set; }
        public string? Category { get; set; }
    }
}

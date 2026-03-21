namespace SocialMotive.Core.Data.Generator
{
    public class DbTemplate
    {
        public int TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Tags { get; set; }
        public string? Category { get; set; }
        public int UserId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsTemplate { get; set; }
        public string? TemplateJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public ICollection<DbRenderJob> RenderJobs { get; set; } = new List<DbRenderJob>();
    }
}

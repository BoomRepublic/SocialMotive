namespace SocialMotive.Core.Data.Generator
{
    public class DbRenderJob
    {
        public int RenderJobId { get; set; }
        public int TemplateId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public byte[]? ImagePng { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public DbTemplate? Template { get; set; }
        public DbUser? User { get; set; }
    }
}

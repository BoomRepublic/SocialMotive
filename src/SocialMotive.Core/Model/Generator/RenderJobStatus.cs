namespace SocialMotive.Core.Model.Generator
{
    public class RenderJobStatus
    {
        public int RenderJobId { get; set; }
        public int TemplateId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

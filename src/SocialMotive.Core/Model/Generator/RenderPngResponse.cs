namespace SocialMotive.Core.Model.Generator
{
    public class RenderPngResponse
    {
        public int RenderJobId { get; set; }
        public string Status { get; set; } = string.Empty;
        public byte[]? ImagePng { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

namespace SocialMotive.Core.Model.Telegram
{
    /// <summary>
    /// Response DTO for Telegram link status
    /// </summary>
    public class LinkStatus
    {
        public bool IsLinked { get; set; }
        public string? TelegramUsername { get; set; }
        public string? ExternalId { get; set; }
        public bool Verified { get; set; }
        public DateTime? LinkedAt { get; set; }
    }
}

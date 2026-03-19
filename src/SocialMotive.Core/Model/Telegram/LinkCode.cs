namespace SocialMotive.Core.Model.Telegram
{
    /// <summary>
    /// Response DTO for generating a Telegram link code
    /// </summary>
    public class LinkCode
    {
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}

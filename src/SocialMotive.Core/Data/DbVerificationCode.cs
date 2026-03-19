namespace SocialMotive.Core.Data
{
    public class DbVerificationCode
    {
        public int VerificationCodeId { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CodeType { get; set; } = string.Empty;
        public string? Target { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
    }
}

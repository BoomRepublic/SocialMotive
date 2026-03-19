namespace SocialMotive.Core.Data
{
    public class DbUserSocialAccount
    {
        public int UserSocialAccountId { get; set; }
        public int? UserId { get; set; }
        public int? SocialPlatformId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? ExternalId { get; set; }
        public bool Verified { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public DbSocialPlatform? SocialPlatform { get; set; }
    }
}

namespace SocialMotive.Core.Data
{
    public class DbSocialPlatform
    {
        public int SocialPlatformId { get; set; }
        public string? Name { get; set; }

        // Navigation properties
        public ICollection<DbUserSocialAccount> UserSocialAccounts { get; set; } = new List<DbUserSocialAccount>();
    }
}

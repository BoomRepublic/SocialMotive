namespace SocialMotive.Core.Data
{
    public class DbUserGroup
    {
        public int UserGroupId { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public DbGroup? Group { get; set; }
    }
}

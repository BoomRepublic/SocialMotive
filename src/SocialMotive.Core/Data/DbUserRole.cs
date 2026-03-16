namespace SocialMotive.Core.Data
{
    public class DbUserRole
    {
        public int UserRoleId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public DbRole? Role { get; set; }
    }
}

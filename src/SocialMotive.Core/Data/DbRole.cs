namespace SocialMotive.Core.Data
{
    public class DbRole
    {
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? HexColor { get; set; }

        // Navigation properties
        public ICollection<DbUserRole> UserRoles { get; set; } = new List<DbUserRole>();
    }
}

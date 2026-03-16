namespace SocialMotive.Core.Data
{
    public class DbOrganizationRole
    {
        public int OrganizationRoleId { get; set; }
        public string? Name { get; set; }
        public string? ColorHex { get; set; }
        public string? BgColorHex { get; set; }

        // Navigation properties
        public ICollection<DbOrganizationUser> OrganizationUsers { get; set; } = new List<DbOrganizationUser>();
    }
}

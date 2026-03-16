namespace SocialMotive.Core.Data
{
    public class DbOrganization
    {
        public int OrganizationId { get; set; }
        public string? Name { get; set; }
        public int? OwnedBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation properties
        public DbUser? Owner { get; set; }
        public DbUser? Creator { get; set; }
        public DbUser? Modifier { get; set; }
        public ICollection<DbOrganizationUser> OrganizationUsers { get; set; } = new List<DbOrganizationUser>();
    }
}

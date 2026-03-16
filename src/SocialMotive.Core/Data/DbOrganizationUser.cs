namespace SocialMotive.Core.Data
{
    public class DbOrganizationUser
    {
        public int OrganizationUserId { get; set; }
        public int? UserId { get; set; }
        public int? OrganizationRoleId { get; set; }
        public int? AssignedBy { get; set; }
        public DateTime? AssingedAt { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public DbOrganizationRole? OrganizationRole { get; set; }
        public DbUser? AssignedByUser { get; set; }
    }
}

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for OrganizationUser management in admin interface
    /// </summary>
    public class OrganizationUser
    {
        public int OrganizationUserId { get; set; }

        public int? UserId { get; set; }

        public int? OrganizationRoleId { get; set; }

        public int? AssignedBy { get; set; }

        public DateTime? AssingedAt { get; set; }
    }
}

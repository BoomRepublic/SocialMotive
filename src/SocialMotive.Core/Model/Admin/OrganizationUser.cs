using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for OrganizationUser management in admin interface
    /// </summary>
    [SwaggerSchema("AdminOrganizationUser")]
    public class OrganizationUser
    {
        public int OrganizationUserId { get; set; }

        public int? UserId { get; set; }

        public int? OrganizationRoleId { get; set; }

        public int? AssignedBy { get; init; }

        public DateTime? AssingedAt { get; init; }
    }
}

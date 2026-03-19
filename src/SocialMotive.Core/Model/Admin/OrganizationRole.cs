using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for OrganizationRole management in admin interface
    /// </summary>
    [SwaggerSchema("AdminOrganizationRole")]
    public class OrganizationRole
    {
        public int OrganizationRoleId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? ColorHex { get; set; }

        [StringLength(50)]
        public string? BgColorHex { get; set; }
    }
}

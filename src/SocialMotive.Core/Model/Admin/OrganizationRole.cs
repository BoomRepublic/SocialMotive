using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for OrganizationRole management in admin interface
    /// </summary>
    public class OrganizationRole
    {
        public int OrganizationRoleId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(7)]
        public string? ColorHex { get; set; }

        [StringLength(7)]
        public string? BgColorHex { get; set; }
    }
}

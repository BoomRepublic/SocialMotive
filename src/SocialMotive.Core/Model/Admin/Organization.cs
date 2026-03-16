using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Organization management in admin interface
    /// </summary>
    public class Organization
    {
        public int OrganizationId { get; set; }

        [StringLength(250)]
        public string? Name { get; set; }

        public int? OwnedBy { get; init; }

        public int? CreatedBy { get; init; }

        public int? ModifiedBy { get; init; }

        public DateTime? CreatedAt { get; init; }

        public DateTime? ModifiedAt { get; init; }
    }
}

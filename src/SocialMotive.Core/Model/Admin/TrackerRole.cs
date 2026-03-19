using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for TrackerRole management in admin interface
    /// </summary>
    [SwaggerSchema("AdminTrackerRole")]
    public class TrackerRole
    {
        public int TrackerRoleId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; init; }
    }
}

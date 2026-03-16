using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for EventType management in admin interface
    /// </summary>
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Event type name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(7)]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid hex color code")]
        public string? Color { get; set; }

        public DateTime Created { get; set; }
    }
}

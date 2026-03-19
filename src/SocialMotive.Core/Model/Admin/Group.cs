using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Group management in admin interface
    /// </summary>
    [SwaggerSchema("AdminGroup")]
    public class Group
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ColorHex { get; set; }

        [StringLength(50)]
        public string? BgColorHex { get; set; }

        [StringLength(50)]
        public string? IconType { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool Publish { get; set; }

        public int Level { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime ModifiedAt { get; init; }
    }
}

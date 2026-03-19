using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Label management in admin interface
    /// </summary>
    [SwaggerSchema("AdminLabel")]
    public class Label
    {
        public int LabelId { get; set; }

        [Required(ErrorMessage = "Label name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ColorHex { get; set; }
        [StringLength(50)]
        public string? BgColorHex { get; set; }

        [StringLength(50)]
        public string? IconType { get; set; }

        [StringLength(50)]
        public string? LabelType { get; set; }

        public bool Publish { get; set; }

        [Range(0, 10)]
        public int Level { get; set; }
    }
}

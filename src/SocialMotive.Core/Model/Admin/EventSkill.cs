using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for EventSkill management in admin interface
    /// </summary>
    [SwaggerSchema("AdminEventSkill")]
    public class EventSkill
    {
        public int EventSkillId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        public int? Difficulty { get; set; }

        [StringLength(50)]
        public string? ColorHex { get; set; }

        [StringLength(50)]
        public string? BgColorHex { get; set; }
    }
}

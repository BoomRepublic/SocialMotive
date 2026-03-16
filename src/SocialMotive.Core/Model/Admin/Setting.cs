using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Setting management in admin interface
    /// </summary>
    public class Setting
    {
        public int SettingId { get; set; }

        [Required(ErrorMessage = "Setting key is required")]
        [StringLength(50)]
        public string SettingKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting value is required")]
        [StringLength(100)]
        public string SettingValue { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Scope { get; set; }
    }
}

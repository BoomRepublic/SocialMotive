using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Group management in admin interface
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(7)]
        public string? ColorHex { get; set; }

        [StringLength(7)]
        public string? BgColorHex { get; set; }

        [StringLength(50)]
        public string? IconType { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool Publish { get; set; }

        public int Level { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public string NameDisplay => $"<span class=\"badge\" style=\"color: {ColorHex}; background-color: {BgColorHex};\">{Name}</span>";
    }
}

using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO for volunteer to update their own profile
    /// </summary>
    public class ProfileUpdate
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [StringLength(2000)]
        public string? Bio { get; set; }
    }
}

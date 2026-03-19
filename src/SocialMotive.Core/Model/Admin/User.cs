using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for User management in admin interface
    /// </summary>
    [SwaggerSchema("AdminUser")]
    public class User
    {
        public int UserId { get; set; }

        [StringLength(100)]
        public string? Username { get; set; }

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

        public int? CityId { get; set; }

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        /// <summary>
        /// Full name (read-only, computed from FirstName and LastName)
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}

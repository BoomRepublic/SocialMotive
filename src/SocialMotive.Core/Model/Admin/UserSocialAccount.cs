using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for UserSocialAccount management in admin interface
    /// </summary>
    public class UserSocialAccount
    {
        public int UserSocialAccountId { get; set; }

        public int? UserId { get; set; }

        public int Platform { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(255)]
        public string UserName { get; set; } = string.Empty;

        [StringLength(2048)]
        public string? Url { get; set; }

        public bool Verified { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}

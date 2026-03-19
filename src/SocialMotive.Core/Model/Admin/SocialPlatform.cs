using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for SocialPlatform management in admin interface
    /// </summary>
    [SwaggerSchema("AdminSocialPlatform")]
    public class SocialPlatform
    {
        public int SocialPlatformId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }
    }
}

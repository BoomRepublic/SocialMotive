using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Invite management in admin interface
    /// </summary>
    [SwaggerSchema("AdminInvite")]
    public class Invite
    {
        public int InviteId { get; set; }

        public int? CreatedByTrackerId { get; set; }

        public DateTime? CreatedAt { get; init; }

        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(4000)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? InviteType { get; set; }

        public int? ClaimedByTrackerId { get; set; }
    }
}
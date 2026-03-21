using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Tracker management in admin interface
    /// </summary>
    [SwaggerSchema("AdminTracker")]
    public class Tracker
    {
        public int TrackerId { get; set; }

        [Required(ErrorMessage = "Display name is required")]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Mobile { get; set; }

        [StringLength(20)]
        public string? LicensePlate { get; set; }

        [StringLength(100)]
        public string? InviteName { get; set; }

        public DateTime JoinedAt { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime ModifiedAt { get; init; }

        public int CheckIn { get; set; }

        public DateTime? CheckInTime { get; set; }

        public float? CheckInLat { get; set; }

        public float? CheckInLon { get; set; }

        public int? GroupId { get; set; }

        public int? TrackerRoleId { get; set; }

        public int? CityId { get; set; }

        public bool? IsAdmin { get; set; }

        public int? InviteId { get; set; }

        public int? UserId { get; set; }

        public bool IsActive { get; set; }

        public bool IsLive { get; set; }

        public DateTime? LastUpdateReceivedAt { get; init; }
    }
}

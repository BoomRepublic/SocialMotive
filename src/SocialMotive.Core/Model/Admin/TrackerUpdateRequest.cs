using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for updating Tracker from admin edit page
    /// Excludes read-only fields like TrackerId, CreatedAt, JoinedAt, QrGuid, InviteCode
    /// </summary>
    public class TrackerUpdateRequest
    {
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
    }
}

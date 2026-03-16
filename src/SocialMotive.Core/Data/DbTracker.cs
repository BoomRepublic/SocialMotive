namespace SocialMotive.Core.Data
{
    public class DbTracker
    {
        public int TrackerId { get; set; }
        public int? GroupId { get; set; }
        public int? TrackerRoleId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? LicensePlate { get; set; }
        public Guid InviteCode { get; set; }
        public string? InviteName { get; set; }
        public int? InvitedBy_TrackerId { get; set; }
        public Guid QrGuid { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CheckIn { get; set; }
        public DateTime? CheckInTime { get; set; }
        public float? CheckInLat { get; set; }
        public float? CheckInLon { get; set; }
        public int? CheckInBy_TrackerId { get; set; }
        public int? CheckInByTrackerId { get; set; }
        public int? InvitedByTrackerId { get; set; }
        public int? CityId { get; set; }
        public bool? IsAdmin { get; set; }
        public int? InviteId { get; set; }
        public int? UserId { get; set; }

        // Navigation properties
        public DbGroup? Group { get; set; }
        public DbTrackerRole? TrackerRole { get; set; }
        public DbCity? City { get; set; }
        public DbInvite? Invite { get; set; }
        public ICollection<DbLocation> Locations { get; set; } = new List<DbLocation>();
        public ICollection<DbTrackerLabel> TrackerLabels { get; set; } = new List<DbTrackerLabel>();
    }
}

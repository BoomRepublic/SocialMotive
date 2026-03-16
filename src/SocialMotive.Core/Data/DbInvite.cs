namespace SocialMotive.Core.Data
{
    public class DbInvite
    {
        public int InviteId { get; set; }
        public int? CreatedByTrackerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? InviteType { get; set; }
        public int? ClaimedByTrackerId { get; set; }

        // Navigation properties
        public ICollection<DbTracker> Trackers { get; set; } = new List<DbTracker>();
    }
}

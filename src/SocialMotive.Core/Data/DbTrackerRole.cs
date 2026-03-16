namespace SocialMotive.Core.Data
{
    public class DbTrackerRole
    {
        public int TrackerRoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<DbTracker> Trackers { get; set; } = new List<DbTracker>();
    }
}

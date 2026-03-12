namespace SocialMotive.WebApp.Models
{
    public class TrackerRole
    {
        public int TrackerRoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
    }
}

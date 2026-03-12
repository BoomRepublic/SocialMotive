namespace SocialMotive.WebApp.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
        public string? IconType { get; set; }
        public string? Description { get; set; }
        public bool Publish { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation properties
        public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
    }
}

namespace SocialMotive.Core.Data
{
    public class DbGroup
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
        public string? BgColorHex { get; set; }
        public string? IconType { get; set; }
        public string? Description { get; set; }
        public bool Publish { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation properties
        public ICollection<DbTracker> Trackers { get; set; } = new List<DbTracker>();
        public ICollection<DbUserGroup> UserGroups { get; set; } = new List<DbUserGroup>();
    }
}

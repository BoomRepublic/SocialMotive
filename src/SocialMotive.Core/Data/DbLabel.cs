namespace SocialMotive.Core.Data
{
    public class DbLabel
    {
        public int LabelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
        public string? BgColorHex { get; set; }
        public string? IconType { get; set; }
        public string? LabelType { get; set; }
        public bool Publish { get; set; }
        public int Level { get; set; }

        // Navigation properties
        public ICollection<DbTrackerLabel> TrackerLabels { get; set; } = new List<DbTrackerLabel>();
        public ICollection<DbUserLabel> UserLabels { get; set; } = new List<DbUserLabel>();
    }
}

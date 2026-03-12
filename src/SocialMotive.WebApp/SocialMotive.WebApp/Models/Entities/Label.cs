namespace SocialMotive.WebApp.Models
{
    public class Label
    {
        public int LabelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
        public string? IconType { get; set; }
        public string? LabelType { get; set; }
        public bool Publish { get; set; }
        public int Level { get; set; }

        // Navigation properties
        public ICollection<TrackerLabel> TrackerLabels { get; set; } = new List<TrackerLabel>();
    }
}

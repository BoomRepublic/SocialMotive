namespace SocialMotive.WebApp.Models
{
    public class TrackerLabel
    {
        public int TrackerLabelId { get; set; }
        public int TrackerId { get; set; }
        public int LabelId { get; set; }

        // Navigation properties
        public Tracker? Tracker { get; set; }
        public Label? Label { get; set; }
    }
}

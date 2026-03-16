namespace SocialMotive.Core.Data
{
    public class DbTrackerLabel
    {
        public int TrackerLabelId { get; set; }
        public int TrackerId { get; set; }
        public int LabelId { get; set; }

        // Navigation properties
        public DbTracker? Tracker { get; set; }
        public DbLabel? Label { get; set; }
    }
}

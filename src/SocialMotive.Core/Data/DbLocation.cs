namespace SocialMotive.Core.Data
{
    public class DbLocation
    {
        public long LocationId { get; set; }
        public int TrackerId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? AccuracyMeters { get; set; }
        public double? AltitudeMeters { get; set; }
        public double? SpeedKmh { get; set; }
        public double? HeadingDegrees { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation properties
        public DbTracker? Tracker { get; set; }
    }
}

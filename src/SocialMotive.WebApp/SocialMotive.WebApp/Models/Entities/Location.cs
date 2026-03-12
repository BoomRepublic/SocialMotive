namespace SocialMotive.WebApp.Models
{
    public class Location
    {
        public long LocationId { get; set; }
        public int TrackerId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float? AccuracyMeters { get; set; }
        public float? AltitudeMeters { get; set; }
        public float? SpeedKmh { get; set; }
        public float? HeadingDegrees { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation properties
        public Tracker? Tracker { get; set; }
    }
}

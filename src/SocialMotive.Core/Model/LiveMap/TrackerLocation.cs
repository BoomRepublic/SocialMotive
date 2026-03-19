namespace SocialMotive.Core.Model.LiveMap;

public class TrackerLocation
{
    public int TrackerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public float? AccuracyMeters { get; set; }
    public float? SpeedKmh { get; set; }
    public float? HeadingDegrees { get; set; }
    public DateTime Timestamp { get; set; }
}

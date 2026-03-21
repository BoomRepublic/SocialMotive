namespace SocialMotive.Core.Model.LiveMap;

public class TrackerLocation
{
    public int TrackerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
    public double? SpeedKmh { get; set; }
    public double? HeadingDegrees { get; set; }
    public DateTime Timestamp { get; set; }
}

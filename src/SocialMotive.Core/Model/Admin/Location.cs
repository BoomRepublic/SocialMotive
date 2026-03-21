using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Location management in admin interface
    /// </summary>
    [SwaggerSchema("AdminLocation")]
    public class Location
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

        public DateTime CreatedAt { get; init; }

        public DateTime ModifiedAt { get; init; }
    }
}

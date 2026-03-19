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

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float? AccuracyMeters { get; set; }

        public float? AltitudeMeters { get; set; }

        public float? SpeedKmh { get; set; }

        public float? HeadingDegrees { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime ModifiedAt { get; init; }
    }
}

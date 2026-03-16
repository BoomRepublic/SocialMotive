using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public
{
    /// <summary>
    /// Public-facing event summary for guest users (no sensitive data)
    /// </summary>
    [SwaggerSchema("PublicEvent")]
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string? EventTypeIcon { get; set; }
        public string? EventTypeColor { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? MaxParticipants { get; set; }
        public int ParticipantCount { get; set; }
        public decimal? HoursEstimate { get; set; }
        public string? SkillsRequired { get; set; }
        public string? BenefitsDescription { get; set; }
        public int? RewardPoints { get; set; }
    }
}

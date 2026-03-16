using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public
{
    /// <summary>
    /// Detailed event view for guest users, includes tasks and organizer info
    /// </summary>
    [SwaggerSchema("PublicEventDetail")]
    public class EventDetail
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string? EventTypeIcon { get; set; }
        public string? EventTypeColor { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string? OrganizerBio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? MaxParticipants { get; set; }
        public int? MinParticipants { get; set; }
        public int ParticipantCount { get; set; }
        public decimal? HoursEstimate { get; set; }
        public string? SkillsRequired { get; set; }
        public string? BenefitsDescription { get; set; }
        public int? RewardPoints { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<EventTask> Tasks { get; set; } = new();
    }
}

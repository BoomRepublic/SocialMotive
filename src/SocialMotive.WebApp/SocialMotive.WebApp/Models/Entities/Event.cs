namespace SocialMotive.WebApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public int Status { get; set; }
        public Guid OrganizerId { get; set; }
        public byte[]? ProfileImage { get; set; }
        public byte[]? CoverImage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Address { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string? City { get; set; }
        public int? MaxParticipants { get; set; }
        public int? MinParticipants { get; set; }
        public decimal? HoursEstimate { get; set; }
        public string? SkillsRequired { get; set; }
        public string? BenefitsDescription { get; set; }
        public int? RewardPoints { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        public EventType? EventType { get; set; }
        public User? Organizer { get; set; }
        public ICollection<EventTask> EventTasks { get; set; } = new List<EventTask>();
        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    }
}

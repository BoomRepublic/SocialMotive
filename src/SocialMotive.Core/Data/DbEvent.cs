namespace SocialMotive.Core.Data
{
    public class DbEvent
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public int Status { get; set; }
        public int? OrganizerId { get; set; }
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
        public DbEventType? EventType { get; set; }
        public DbUser? Organizer { get; set; }
        public ICollection<DbEventTask> EventTasks { get; set; } = new List<DbEventTask>();
        public ICollection<DbEventParticipant> Participants { get; set; } = new List<DbEventParticipant>();
    }
}

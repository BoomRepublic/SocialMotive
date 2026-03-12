namespace SocialMotive.WebApp.Models
{
    public class EventParticipant
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Guid UserId { get; set; }
        public int Status { get; set; }
        public decimal? HoursWorked { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public User? User { get; set; }
    }
}

namespace SocialMotive.Core.Data
{
    public class DbEventParticipant
    {
        public int EventParticipantId { get; set; }
        public int EventId { get; set; }
        public int? UserId { get; set; }
        public int Status { get; set; }
        public decimal? HoursWorked { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public DbEvent? Event { get; set; }
        public DbUser? User { get; set; }
    }
}

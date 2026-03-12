namespace SocialMotive.WebApp.Models
{
    public class EventTaskAssignment
    {
        public int Id { get; set; }
        public int EventTaskId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // Navigation properties
        public EventTask? EventTask { get; set; }
        public User? User { get; set; }
    }
}

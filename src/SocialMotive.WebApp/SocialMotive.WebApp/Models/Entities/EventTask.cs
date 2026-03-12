namespace SocialMotive.WebApp.Models
{
    public class EventTask
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Difficulty { get; set; }
        public bool Required { get; set; }
        public int? MaxParticipants { get; set; }
        public decimal? HoursEstimate { get; set; }
        public int OrderIndex { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public ICollection<EventTaskAssignment> Assignments { get; set; } = new List<EventTaskAssignment>();
    }
}

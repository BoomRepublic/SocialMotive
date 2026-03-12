namespace SocialMotive.WebApp.Models
{
    public class EventType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public DateTime Created { get; set; }

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

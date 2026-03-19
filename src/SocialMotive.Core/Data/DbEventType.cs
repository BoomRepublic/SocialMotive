namespace SocialMotive.Core.Data
{
    public class DbEventType
    {
        public int EventTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? ColorHex { get; set; }
        public string? BgColorHex { get; set; }
        public DateTime Created { get; set; }

        // Navigation properties
        public ICollection<DbEvent> Events { get; set; } = new List<DbEvent>();
    }
}

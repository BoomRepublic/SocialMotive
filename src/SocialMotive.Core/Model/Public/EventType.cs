using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public
{
    /// <summary>
    /// Public-facing event type info for guest users
    /// </summary>
    
    [SwaggerSchema("PublicEventType")]
    public class EventType
    {
        public int EventTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }
}

using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public
{
    /// <summary>
    /// Public-facing event task info for guest users
    /// </summary>
    [SwaggerSchema("PublicEventTask")]
    public class EventTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Difficulty { get; set; }
        public bool Required { get; set; }
        public int? MaxParticipants { get; set; }
        public decimal? HoursEstimate { get; set; }
        public int OrderIndex { get; set; }
    }
}

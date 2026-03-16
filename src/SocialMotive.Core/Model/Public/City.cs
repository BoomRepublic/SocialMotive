using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Public
{
    /// <summary>
    /// Public-facing city info for location filtering
    /// </summary>
    
    [SwaggerSchema("PublicCity")]
    public class City
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string? ProvincieNaam { get; set; }
    }
}

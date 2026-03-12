namespace SocialMotive.WebApp.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string? Code { get; set; }
        public string? CodeGm { get; set; }
        public string? ProvincieNaam { get; set; }
        public string? ProvincieCode { get; set; }
        public string? ProvincieCodePv { get; set; }

        // Navigation properties
        public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}

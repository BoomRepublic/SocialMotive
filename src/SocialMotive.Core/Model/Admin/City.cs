using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for City management in admin interface
    /// </summary>
    [SwaggerSchema("AdminCity")]
    public class City
    {
        public int CityId { get; set; }

        [StringLength(250)]
        public string? Name { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(50)]
        public string? CodeGm { get; set; }

        [StringLength(150)]
        public string? ProvincieNaam { get; set; }

        [StringLength(50)]
        public string? ProvincieCode { get; set; }

        [StringLength(50)]
        public string? ProvincieCodePv { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for Event management in admin interface
    /// </summary>
    [SwaggerSchema("AdminEvent")]
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event type is required")]
        public int EventTypeId { get; set; }

        public int Status { get; set; }

        public int? OrganizerId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxParticipants { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinParticipants { get; set; }

        [Range(0, 999999)]
        public decimal? HoursEstimate { get; set; }

        [StringLength(1000)]
        public string? SkillsRequired { get; set; }

        [StringLength(1000)]
        public string? BenefitsDescription { get; set; }

        [Range(0, int.MaxValue)]
        public int? RewardPoints { get; set; }

        public bool? IsVerified { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }
    }
}

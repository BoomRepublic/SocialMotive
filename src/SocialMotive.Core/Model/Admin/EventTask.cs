using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// DTO for EventTask management in admin interface
    /// </summary>
    [SwaggerSchema("AdminEventTask")]
    public class EventTask
    {
        public int EventTaskId { get; set; }

        public int EventId { get; set; }

        public int? EventSkillId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Difficulty { get; set; }

        public bool Required { get; set; }

        public int? MaxParticipants { get; set; }

        public int? MinParticipants { get; set; }

        public decimal? HoursEstimate { get; set; }

        public int OrderIndex { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; init; }
    }
}

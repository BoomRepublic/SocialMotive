using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO for volunteer to submit a review/rating for an event they participated in
    /// </summary>
    public class ParticipantReviewUpdate
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        [StringLength(2000)]
        public string? Review { get; set; }
    }
}

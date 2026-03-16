namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO representing the volunteer's participation in an event
    /// </summary>
    public class Participation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public int Status { get; set; }
        public decimal? HoursWorked { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

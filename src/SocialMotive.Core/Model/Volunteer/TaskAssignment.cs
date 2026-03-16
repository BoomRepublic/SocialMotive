namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO representing a volunteer's task assignment
    /// </summary>
    public class TaskAssignment
    {
        public int Id { get; set; }
        public int EventTaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}

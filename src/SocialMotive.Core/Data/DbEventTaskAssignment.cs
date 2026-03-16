namespace SocialMotive.Core.Data
{
    public class DbEventTaskAssignment
    {
        public int EventTaskAssignmentId { get; set; }
        public int EventTaskId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // Navigation properties
        public DbEventTask? EventTask { get; set; }
        public DbUser? User { get; set; }
    }
}

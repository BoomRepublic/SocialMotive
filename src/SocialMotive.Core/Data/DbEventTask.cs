namespace SocialMotive.Core.Data
{
    public class DbEventTask
    {
        public int EventTaskId { get; set; }
        public int EventId { get; set; }
        public int? EventSkillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Difficulty { get; set; }
        public bool Required { get; set; }
        public int? MaxParticipants { get; set; }
        public int? MinParticipants { get; set; }
        public decimal? HoursEstimate { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        // Navigation properties
        public DbEvent? Event { get; set; }
        public DbEventSkill? EventSkill { get; set; }
        public ICollection<DbEventTaskAssignment> Assignments { get; set; } = new List<DbEventTaskAssignment>();
    }
}

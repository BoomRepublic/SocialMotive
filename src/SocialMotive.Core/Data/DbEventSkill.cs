namespace SocialMotive.Core.Data
{
    public class DbEventSkill
    {
        public int EventSkillId { get; set; }
        public string? Name { get; set; }
        public int? Difficulty { get; set; }
        public string? ColorHex { get; set; }
        public string? BgColorHex { get; set; }

        // Navigation properties
        public ICollection<DbEventTask> EventTasks { get; set; } = new List<DbEventTask>();
    }
}

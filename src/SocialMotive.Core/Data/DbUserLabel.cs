namespace SocialMotive.Core.Data
{
    public class DbUserLabel
    {
        public int UserLabelId { get; set; }
        public int? UserId { get; set; }
        public int? LabelId { get; set; }

        // Navigation properties
        public DbUser? User { get; set; }
        public DbLabel? Label { get; set; }
    }
}

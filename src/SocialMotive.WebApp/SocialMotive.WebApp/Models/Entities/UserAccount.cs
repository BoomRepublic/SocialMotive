namespace SocialMotive.WebApp.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int Platform { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Url { get; set; }
        public bool Verified { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public User? User { get; set; }
    }
}

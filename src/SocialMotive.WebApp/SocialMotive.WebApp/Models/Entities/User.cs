namespace SocialMotive.WebApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? MobilePhone { get; set; }
        public byte[]? ProfileImage { get; set; }
        public byte[]? CoverImage { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public ICollection<Event> OrganizerEvents { get; set; } = new List<Event>();
        public ICollection<EventParticipant> Participations { get; set; } = new List<EventParticipant>();
        public ICollection<EventTaskAssignment> TaskAssignments { get; set; } = new List<EventTaskAssignment>();
        public ICollection<UserAccount> Accounts { get; set; } = new List<UserAccount>();
    }
}

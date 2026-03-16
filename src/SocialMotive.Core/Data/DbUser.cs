namespace SocialMotive.Core.Data
{
    public class DbUser
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int? CityId { get; set; }
        public string? MobilePhone { get; set; }
        public byte[]? ProfileImage { get; set; }
        public byte[]? CoverImage { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public ICollection<DbEvent> OrganizerEvents { get; set; } = new List<DbEvent>();
        public ICollection<DbEventParticipant> Participations { get; set; } = new List<DbEventParticipant>();
        public ICollection<DbEventTaskAssignment> TaskAssignments { get; set; } = new List<DbEventTaskAssignment>();
        public ICollection<DbUserSocialAccount> SocialAccounts { get; set; } = new List<DbUserSocialAccount>();
        public ICollection<DbUserGroup> UserGroups { get; set; } = new List<DbUserGroup>();
        public ICollection<DbUserLabel> UserLabels { get; set; } = new List<DbUserLabel>();
        public ICollection<DbUserRole> UserRoles { get; set; } = new List<DbUserRole>();
    }
}

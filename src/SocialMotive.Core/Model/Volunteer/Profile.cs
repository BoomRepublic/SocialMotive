namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO representing the volunteer's own profile (read)
    /// </summary>
    public class Profile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MobilePhone { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
    }
}

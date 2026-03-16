using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public;

[SwaggerSchema("PublicUserProfile")]
public class UserProfile
{
    public string Username { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int EventsAttended { get; set; }
    public double HoursVolunteered { get; set; }
    public List<UserEventSummary>? RecentEvents { get; set; }
}

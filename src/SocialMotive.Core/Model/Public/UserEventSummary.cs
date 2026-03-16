using Swashbuckle.AspNetCore.Annotations;
namespace SocialMotive.Core.Model.Public;
    
[SwaggerSchema("PublicUserEventSummary")]
public class UserEventSummary
{
    public string Title { get; set; } = default!;
    public DateTime Date { get; set; }
}

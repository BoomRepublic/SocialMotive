using SocialMotive.Core.Data;
using SocialMotive.Core.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    /// <summary>
    /// Response DTO for Tracker update operations
    /// Inherits from Response&lt;Tracker&gt; for standardized API responses with strong typing
    /// </summary>
    [SwaggerSchema("AdminTrackerUpdateResponse")]
    public class TrackerUpdateResponse : Response<Tracker>
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Organizer Controller for authenticated organizer users.
/// </summary>
[ApiController]
[Route("api/organizer")]
[Authorize(Roles = AppRoles.Organizer + "," + AppRoles.Admin)]
public class OrganizerController : ControllerBase
{
    private readonly ILogger<OrganizerController> _logger;
    private readonly SocialMotiveDbContext _dbContext;

    public OrganizerController(
        ILogger<OrganizerController> logger,
        SocialMotiveDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}

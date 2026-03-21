using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialMotive.Core.Model.LiveMap;
using SocialMotive.LiveMap.Hubs;
using SocialMotive.Core.Services;

namespace SocialMotive.LiveMap.Controllers;

/// <summary>
/// Callback endpoint called by TelegramBot after storing a new location.
/// Updates the in-memory cache and broadcasts to all SignalR clients.
/// </summary>
[ApiController]
[Route("api")]
public class LocationCallbackController : ControllerBase
{
    private readonly LocationCacheService _cache;
    private readonly IHubContext<LocationHub> _hubContext;
    private readonly ILogger<LocationCallbackController> _logger;

    public LocationCallbackController(
        LocationCacheService cache,
        IHubContext<LocationHub> hubContext,
        ILogger<LocationCallbackController> logger)
    {
        _cache = cache;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Receives a tracker location update and broadcasts it to all connected map clients.
    /// </summary>
    [HttpPost("location-update")]
    public async Task<IActionResult> LocationUpdate([FromBody] TrackerLocation location)
    {
        _cache.Update(location);

        var allLocations = _cache.GetAll();
        await _hubContext.Clients.All.SendAsync("ReceiveLocations", allLocations);

        _logger.LogDebug("Location update broadcast for tracker {TrackerId}", location.TrackerId);

        return Ok();
    }
}

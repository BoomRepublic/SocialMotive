using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Public;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Public Controller for guest (unauthenticated) users.
/// Provides read-only access to published events, event types, and cities.
/// No authorization required.
/// </summary>
[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly ILogger<PublicController> _logger;
    private readonly SocialMotiveDbContext _dbContext;
    private readonly IMapper _mapper;

    public PublicController(
        ILogger<PublicController> logger,
        SocialMotiveDbContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    #region Events

    /// <summary>
    /// Get list of upcoming published events
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<List<Event>>> GetEvents([FromQuery] string? city = null, [FromQuery] int? eventTypeId = null)
    {
        try
        {
            var query = _dbContext.Events
                .Where(e => e.PublishedAt != null && e.EndDate >= DateTime.UtcNow)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(e => e.City != null && e.City.Contains(city));
            }

            if (eventTypeId.HasValue)
            {
                query = query.Where(e => e.EventTypeId == eventTypeId.Value);
            }

            var events = await query
                .OrderBy(e => e.StartDate)
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public events");
            return BadRequest(new { message = "Error retrieving events", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single published event by ID with full details including tasks
    /// </summary>
    [HttpGet("events/{id:int}")]
    public async Task<ActionResult<EventDetail>> GetEvent(int id)
    {
        try
        {
            var detail = await _dbContext.Events
                .Where(e => e.EventId == id && e.PublishedAt != null)
                .ProjectTo<EventDetail>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (detail == null)
                return NotFound();

            return Ok(detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public event {EventId}", id);
            return BadRequest(new { message = "Error retrieving event", error = ex.Message });
        }
    }

    #endregion

    #region Event Types

    /// <summary>
    /// Get list of event types for filtering
    /// </summary>
    [HttpGet("event-types")]
    public async Task<ActionResult<List<EventType>>> GetEventTypes()
    {
        try
        {
            var eventTypes = await _dbContext.EventTypes
                .OrderBy(et => et.Name)
                .ProjectTo<EventType>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(eventTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public event types");
            return BadRequest(new { message = "Error retrieving event types", error = ex.Message });
        }
    }

    #endregion

    #region Users

    /// <summary>
    /// Get public profile for a user by username
    /// </summary>
    [HttpGet("users/{username}")]
    [HttpGet("profile/{username}")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(string username)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(u => u.Participations)
                    .ThenInclude(p => p.Event)
                .FirstOrDefaultAsync(u => u.Username != null && u.Username.ToLower() == username.ToLower());

            if (user == null)
                return NotFound();

            var completedParticipations = user.Participations
                .Where(p => p.CompletedAt != null)
                .ToList();

            var profile = new UserProfile
            {
                Username = user.Username ?? string.Empty,
                DisplayName = $"{user.FirstName} {user.LastName}".Trim(),
                Bio = user.Bio,
                EventsAttended = completedParticipations.Count,
                HoursVolunteered = (double)completedParticipations.Sum(p => p.HoursWorked ?? 0),
                RecentEvents = completedParticipations
                    .Where(p => p.Event != null)
                    .OrderByDescending(p => p.Event!.StartDate)
                    .Take(5)
                    .Select(p => new UserEventSummary
                    {
                        Title = p.Event!.Title,
                        Date = p.Event.StartDate
                    })
                    .ToList()
            };

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public profile for {Username}", username);
            return BadRequest(new { message = "Error retrieving profile", error = ex.Message });
        }
    }

    #endregion

    #region Cities

    /// <summary>
    /// Get list of cities for location-based filtering
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<List<City>>> GetCities()
    {
        try
        {
            var cities = await _dbContext.Cities
                .OrderBy(c => c.Name)
                .ProjectTo<City>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public cities");
            return BadRequest(new { message = "Error retrieving cities", error = ex.Message });
        }
    }

    #endregion
}

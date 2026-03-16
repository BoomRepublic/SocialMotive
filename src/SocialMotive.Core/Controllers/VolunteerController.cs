using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Volunteer;
using Profile = SocialMotive.Core.Model.Volunteer.Profile;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Volunteer Controller for authenticated volunteer users.
/// Provides endpoints for managing the volunteer's own profile,
/// event participation (join/leave), reviews, and task assignments.
/// All operations are scoped to the authenticated user.
/// </summary>
[ApiController]
[Route("api/volunteer")]
[Authorize(Roles = AppRoles.Volunteer + "," + AppRoles.Organizer + "," + AppRoles.Admin)]
public class VolunteerController : ControllerBase
{
    private readonly ILogger<VolunteerController> _logger;
    private readonly SocialMotiveDbContext _dbContext;
    private readonly IMapper _mapper;

    public VolunteerController(
        ILogger<VolunteerController> logger,
        SocialMotiveDbContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Extract the authenticated user's ID from claims
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("UserId")?.Value;

        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found in claims");
    }

    #region Profile

    /// <summary>
    /// Get the authenticated volunteer's own profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<Profile>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<Profile>(user));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting volunteer profile");
            return BadRequest(new { message = "Error retrieving profile", error = ex.Message });
        }
    }

    /// <summary>
    /// Update the authenticated volunteer's own profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<Profile>> UpdateProfile([FromBody] ProfileUpdate update)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            _mapper.Map(update, user);
            user.Modified = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Profile>(user));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating volunteer profile");
            return BadRequest(new { message = "Error updating profile", error = ex.Message });
        }
    }

    #endregion

    #region Event Participation

    /// <summary>
    /// Get the authenticated volunteer's event participations
    /// </summary>
    [HttpGet("participations")]
    public async Task<ActionResult<List<Participation>>> GetParticipations()
    {
        try
        {
            var userId = GetCurrentUserId();

            var participations = await _dbContext.EventParticipants
                .Where(ep => ep.UserId == userId)
                .OrderByDescending(ep => ep.JoinedAt)
                .ProjectTo<Participation>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(participations);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting volunteer participations");
            return BadRequest(new { message = "Error retrieving participations", error = ex.Message });
        }
    }

    /// <summary>
    /// Join an event (create participation record with status = registered)
    /// </summary>
    [HttpPost("participations/{eventId:int}")]
    public async Task<ActionResult<Participation>> JoinEvent(int eventId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var ev = await _dbContext.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.PublishedAt != null);

            if (ev == null)
                return NotFound(new { message = "Event not found or not published" });

            var existing = await _dbContext.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);

            if (existing != null)
                return BadRequest(new { message = "Already registered for this event" });

            if (ev.MaxParticipants.HasValue && ev.Participants.Count >= ev.MaxParticipants.Value)
                return BadRequest(new { message = "Event has reached maximum participants" });

            var participation = new DbEventParticipant
            {
                EventId = eventId,
                UserId = userId,
                Status = 0, // Registered
                JoinedAt = DateTime.UtcNow
            };

            _dbContext.EventParticipants.Add(participation);
            await _dbContext.SaveChangesAsync();

            participation.Event = ev;
            var result = _mapper.Map<Participation>(participation);

            return CreatedAtAction(nameof(GetParticipations), result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining event {EventId}", eventId);
            return BadRequest(new { message = "Error joining event", error = ex.Message });
        }
    }

    /// <summary>
    /// Leave an event (delete participation record)
    /// </summary>
    [HttpDelete("participations/{eventId:int}")]
    public async Task<ActionResult> LeaveEvent(int eventId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var participation = await _dbContext.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);

            if (participation == null)
                return NotFound(new { message = "Not registered for this event" });

            _dbContext.EventParticipants.Remove(participation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Successfully left the event" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving event {EventId}", eventId);
            return BadRequest(new { message = "Error leaving event", error = ex.Message });
        }
    }

    /// <summary>
    /// Update review and rating for an event the volunteer participated in
    /// </summary>
    [HttpPut("participations/{eventId:int}/review")]
    public async Task<ActionResult<Participation>> UpdateReview(int eventId, [FromBody] ParticipantReviewUpdate update)
    {
        try
        {
            var userId = GetCurrentUserId();

            var participation = await _dbContext.EventParticipants
                .Include(ep => ep.Event)
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);

            if (participation == null)
                return NotFound(new { message = "Not registered for this event" });

            participation.Rating = update.Rating;
            participation.Review = update.Review;

            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Participation>(participation));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review for event {EventId}", eventId);
            return BadRequest(new { message = "Error updating review", error = ex.Message });
        }
    }

    #endregion

    #region Task Assignments

    /// <summary>
    /// Get the authenticated volunteer's task assignments
    /// </summary>
    [HttpGet("task-assignments")]
    public async Task<ActionResult<List<TaskAssignment>>> GetTaskAssignments()
    {
        try
        {
            var userId = GetCurrentUserId();

            var assignments = await _dbContext.EventTaskAssignments
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartTime)
                .ProjectTo<TaskAssignment>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(assignments);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting volunteer task assignments");
            return BadRequest(new { message = "Error retrieving task assignments", error = ex.Message });
        }
    }

    /// <summary>
    /// Sign up for an event task
    /// </summary>
    [HttpPost("task-assignments/{eventTaskId:int}")]
    public async Task<ActionResult<TaskAssignment>> SignUpForTask(int eventTaskId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var task = await _dbContext.EventTasks
                .Include(t => t.Event)
                .Include(t => t.Assignments)
                .FirstOrDefaultAsync(t => t.EventTaskId == eventTaskId);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            if (task.Event?.PublishedAt == null)
                return BadRequest(new { message = "Event is not published" });

            var isParticipant = await _dbContext.EventParticipants
                .AnyAsync(ep => ep.EventId == task.EventId && ep.UserId == userId);

            if (!isParticipant)
                return BadRequest(new { message = "Must be registered for the event to sign up for tasks" });

            var existing = await _dbContext.EventTaskAssignments
                .FirstOrDefaultAsync(a => a.EventTaskId == eventTaskId && a.UserId == userId);

            if (existing != null)
                return BadRequest(new { message = "Already assigned to this task" });

            if (task.MaxParticipants.HasValue && task.Assignments.Count >= task.MaxParticipants.Value)
                return BadRequest(new { message = "Task has reached maximum participants" });

            var assignment = new DbEventTaskAssignment
            {
                EventTaskId = eventTaskId,
                UserId = userId,
                StartTime = task.Event!.StartDate,
                EndTime = task.Event.EndDate,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _dbContext.EventTaskAssignments.Add(assignment);
            await _dbContext.SaveChangesAsync();

            assignment.EventTask = task;

            return CreatedAtAction(nameof(GetTaskAssignments), _mapper.Map<TaskAssignment>(assignment));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up for task {EventTaskId}", eventTaskId);
            return BadRequest(new { message = "Error signing up for task", error = ex.Message });
        }
    }

    /// <summary>
    /// Update notes on a task assignment
    /// </summary>
    [HttpPut("task-assignments/{eventTaskId:int}")]
    public async Task<ActionResult<TaskAssignment>> UpdateTaskAssignment(int eventTaskId, [FromBody] TaskAssignmentUpdate update)
    {
        try
        {
            var userId = GetCurrentUserId();

            var assignment = await _dbContext.EventTaskAssignments
                .Include(a => a.EventTask)
                    .ThenInclude(t => t!.Event)
                .FirstOrDefaultAsync(a => a.EventTaskId == eventTaskId && a.UserId == userId);

            if (assignment == null)
                return NotFound(new { message = "Task assignment not found" });

            assignment.Notes = update.Notes;
            assignment.Updated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<TaskAssignment>(assignment));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task assignment {EventTaskId}", eventTaskId);
            return BadRequest(new { message = "Error updating task assignment", error = ex.Message });
        }
    }

    /// <summary>
    /// Withdraw from a task assignment
    /// </summary>
    [HttpDelete("task-assignments/{eventTaskId:int}")]
    public async Task<ActionResult> WithdrawFromTask(int eventTaskId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var assignment = await _dbContext.EventTaskAssignments
                .FirstOrDefaultAsync(a => a.EventTaskId == eventTaskId && a.UserId == userId);

            if (assignment == null)
                return NotFound(new { message = "Task assignment not found" });

            _dbContext.EventTaskAssignments.Remove(assignment);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Successfully withdrew from the task" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing from task {EventTaskId}", eventTaskId);
            return BadRequest(new { message = "Error withdrawing from task", error = ex.Message });
        }
    }

    #endregion
}

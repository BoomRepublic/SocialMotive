using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Model.Admin;
using SocialMotive.Core.Data;
using Microsoft.Extensions.Logging;

namespace SocialMotive.Core.Controllers.Admin;

public partial class AdminController
{
    #region Events

    /// <summary>
    /// Get list of events
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        try
        {
            var events = await _dbContext.Events
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events");
            return BadRequest(new { message = "Error retrieving events", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single event by ID
    /// </summary>
    [HttpGet("event/{id:int}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        try
        {
            var @event = await _dbContext.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            return Ok(_mapper.Map<Event>(@event));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event {EventId}", id);
            return BadRequest(new { message = "Error retrieving event", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new event
    /// </summary>
    [HttpPost("event")]
    public async Task<ActionResult<Event>> CreateEvent([FromBody] Event Event)
    {
        try
        {
            var @event = _mapper.Map<DbEvent>(Event);
            @event.CreatedAt = DateTime.UtcNow;
            @event.UpdatedAt = DateTime.UtcNow;

            _dbContext.Events.Add(@event);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = @event.EventId }, _mapper.Map<Event>(@event));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return BadRequest(new { message = "Error creating event", error = ex.Message });
        }
    }

    /// <summary>
    /// Update event
    /// </summary>
    [HttpPut("event/{id:int}")]
    public async Task<ActionResult<Event>> UpdateEvent(int id, [FromBody] Event Event)
    {
        try
        {
            var @event = await _dbContext.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            _mapper.Map(Event, @event);
            @event.UpdatedAt = DateTime.UtcNow;

            _dbContext.Events.Update(@event);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Event>(@event));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            return BadRequest(new { message = "Error updating event", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete event
    /// </summary>
    [HttpDelete("event/{id:int}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        try
        {
            var @event = await _dbContext.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            _dbContext.Events.Remove(@event);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            return BadRequest(new { message = "Error deleting event", error = ex.Message });
        }
    }

    #endregion
}

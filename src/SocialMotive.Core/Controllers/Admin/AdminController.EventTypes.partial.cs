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
    #region EventTypes

    /// <summary>
    /// Get list of event types
    /// </summary>
    [HttpGet("event-types")]
    public async Task<ActionResult<List<EventType>>> GetEventTypes()
    {
        try
        {
            var eventTypes = await _dbContext.EventTypes
                .ProjectTo<EventType>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(eventTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event types");
            return BadRequest(new { message = "Error retrieving event types", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single event type by ID
    /// </summary>
    [HttpGet("event-type/{id:int}")]
    public async Task<ActionResult<EventType>> GetEventType(int id)
    {
        try
        {
            var eventType = await _dbContext.EventTypes.FindAsync(id);
            if (eventType == null)
                return NotFound();

            return Ok(_mapper.Map<EventType>(eventType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event type {EventTypeId}", id);
            return BadRequest(new { message = "Error retrieving event type", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new event type
    /// </summary>
    [HttpPost("event-type")]
    public async Task<ActionResult<EventType>> CreateEventType([FromBody] EventType EventType)
    {
        try
        {
            var eventType = _mapper.Map<DbEventType>(EventType);
            eventType.Created = DateTime.UtcNow;

            _dbContext.EventTypes.Add(eventType);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventType), new { id = eventType.EventTypeId }, _mapper.Map<EventType>(eventType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event type");
            return BadRequest(new { message = "Error creating event type", error = ex.Message });
        }
    }

    /// <summary>
    /// Update event type
    /// </summary>
    [HttpPut("event-type/{id:int}")]
    public async Task<ActionResult<EventType>> UpdateEventType(int id, [FromBody] EventType EventType)
    {
        try
        {
            var eventType = await _dbContext.EventTypes.FindAsync(id);
            if (eventType == null)
                return NotFound();

            _mapper.Map(EventType, eventType);

            _dbContext.EventTypes.Update(eventType);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<EventType>(eventType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event type {EventTypeId}", id);
            return BadRequest(new { message = "Error updating event type", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete event type
    /// </summary>
    [HttpDelete("event-type/{id:int}")]
    public async Task<IActionResult> DeleteEventType(int id)
    {
        try
        {
            var eventType = await _dbContext.EventTypes.FindAsync(id);
            if (eventType == null)
                return NotFound();

            _dbContext.EventTypes.Remove(eventType);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event type {EventTypeId}", id);
            return BadRequest(new { message = "Error deleting event type", error = ex.Message });
        }
    }

    #endregion
}

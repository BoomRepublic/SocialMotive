using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Model.Admin;
using SocialMotive.Core.Data;

namespace SocialMotive.Core.Controllers.Admin;

public partial class AdminController
{
    #region EventTasks

    /// <summary>
    /// Get list of event tasks
    /// </summary>
    [HttpGet("event-tasks")]
    public async Task<ActionResult<List<EventTask>>> GetEventTasks()
    {
        try
        {
            var tasks = await _dbContext.EventTasks
                .ProjectTo<EventTask>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event tasks");
            return BadRequest(new { message = "Error retrieving event tasks", error = ex.Message });
        }
    }

    /// <summary>
    /// Get event tasks by event ID
    /// </summary>
    [HttpGet("event/{eventId:int}/tasks")]
    public async Task<ActionResult<List<EventTask>>> GetEventTasksByEvent(int eventId)
    {
        try
        {
            var tasks = await _dbContext.EventTasks
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.OrderIndex)
                .ProjectTo<EventTask>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for event {EventId}", eventId);
            return BadRequest(new { message = "Error retrieving event tasks", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single event task by ID
    /// </summary>
    [HttpGet("event-task/{id:int}")]
    public async Task<ActionResult<EventTask>> GetEventTask(int id)
    {
        try
        {
            var task = await _dbContext.EventTasks.FindAsync(id);
            if (task == null)
                return NotFound();

            return Ok(_mapper.Map<EventTask>(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event task {EventTaskId}", id);
            return BadRequest(new { message = "Error retrieving event task", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new event task
    /// </summary>
    [HttpPost("event-task")]
    public async Task<ActionResult<EventTask>> CreateEventTask([FromBody] EventTask dto)
    {
        try
        {
            var task = _mapper.Map<DbEventTask>(dto);
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _dbContext.EventTasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventTask), new { id = task.EventTaskId }, _mapper.Map<EventTask>(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event task");
            return BadRequest(new { message = "Error creating event task", error = ex.Message });
        }
    }

    /// <summary>
    /// Update event task
    /// </summary>
    [HttpPut("event-task/{id:int}")]
    public async Task<ActionResult<EventTask>> UpdateEventTask(int id, [FromBody] EventTask dto)
    {
        try
        {
            var task = await _dbContext.EventTasks.FindAsync(id);
            if (task == null)
                return NotFound();

            _mapper.Map(dto, task);
            task.UpdatedAt = DateTime.UtcNow;

            _dbContext.EventTasks.Update(task);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<EventTask>(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event task {EventTaskId}", id);
            return BadRequest(new { message = "Error updating event task", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete event task
    /// </summary>
    [HttpDelete("event-task/{id:int}")]
    public async Task<IActionResult> DeleteEventTask(int id)
    {
        try
        {
            var task = await _dbContext.EventTasks.FindAsync(id);
            if (task == null)
                return NotFound();

            _dbContext.EventTasks.Remove(task);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event task {EventTaskId}", id);
            return BadRequest(new { message = "Error deleting event task", error = ex.Message });
        }
    }

    #endregion
}

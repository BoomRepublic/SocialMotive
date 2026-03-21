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
    #region Trackers

    /// <summary>
    /// Get list of trackers
    /// </summary>
    [HttpGet("trackers")]
    public async Task<ActionResult<List<Tracker>>> GetTrackers()
    {
        try
        {
            var trackers = await _dbContext.Trackers
                .ProjectTo<Tracker>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(trackers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trackers");
            return BadRequest(new { message = "Error retrieving trackers", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single tracker by ID
    /// </summary>
    [HttpGet("tracker/{id:int}")]
    public async Task<ActionResult<Tracker>> GetTracker(int id)
    {
        try
        {
            var tracker = await _dbContext.Trackers.FindAsync(id);
            if (tracker == null)
                return NotFound();

            return Ok(_mapper.Map<Tracker>(tracker));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tracker {TrackerId}", id);
            return BadRequest(new { message = "Error retrieving tracker", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new tracker
    /// </summary>
    [HttpPost("tracker")]
    public async Task<ActionResult<Tracker>> CreateTracker([FromBody] Tracker Tracker)
    {
        try
        {
            var tracker = _mapper.Map<DbTracker>(Tracker);
            tracker.CreatedAt = DateTime.UtcNow;
            tracker.ModifiedAt = DateTime.UtcNow;
            tracker.JoinedAt = DateTime.UtcNow;

            _dbContext.Trackers.Add(tracker);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTracker), new { id = tracker.TrackerId }, _mapper.Map<Tracker>(tracker));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tracker");
            return BadRequest(new { message = "Error creating tracker", error = ex.Message });
        }
    }

    /// <summary>
    /// Update tracker
    /// </summary>
    [HttpPut("tracker/{id:int}")]
    public async Task<ActionResult<Tracker>> UpdateTracker(int id, [FromBody] TrackerUpdateRequest Tracker)
    {
        try
        {
            var tracker = await _dbContext.Trackers.FindAsync(id);
            if (tracker == null)
                return NotFound();

            _mapper.Map(Tracker, tracker);
            tracker.ModifiedAt = DateTime.UtcNow;

            _dbContext.Trackers.Update(tracker);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Tracker>(tracker));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tracker {TrackerId}", id);
            return BadRequest(new { message = "Error updating tracker", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete tracker
    /// </summary>
    [HttpDelete("tracker/{id:int}")]
    public async Task<IActionResult> DeleteTracker(int id)
    {
        try
        {
            var tracker = await _dbContext.Trackers.FindAsync(id);
            if (tracker == null)
                return NotFound();

            _dbContext.Trackers.Remove(tracker);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tracker {TrackerId}", id);
            return BadRequest(new { message = "Error deleting tracker", error = ex.Message });
        }
    }

    #endregion
}

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
    #region Locations

    /// <summary>
    /// Get list of locations
    /// </summary>
    [HttpGet("locations")]
    public async Task<ActionResult<List<Location>>> GetLocations()
    {
        try
        {
            var locations = await _dbContext.Locations
                .ProjectTo<Location>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locations");
            return BadRequest(new { message = "Error retrieving locations", error = ex.Message });
        }
    }

    /// <summary>
    /// Get locations by tracker ID
    /// </summary>
    [HttpGet("tracker/{trackerId:int}/locations")]
    public async Task<ActionResult<List<Location>>> GetLocationsByTracker(int trackerId)
    {
        try
        {
            var locations = await _dbContext.Locations
                .Where(l => l.TrackerId == trackerId)
                .OrderByDescending(l => l.Timestamp)
                .ProjectTo<Location>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locations for tracker {TrackerId}", trackerId);
            return BadRequest(new { message = "Error retrieving locations", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single location by ID
    /// </summary>
    [HttpGet("location/{id:long}")]
    public async Task<ActionResult<Location>> GetLocation(long id)
    {
        try
        {
            var location = await _dbContext.Locations.FindAsync(id);
            if (location == null)
                return NotFound();

            return Ok(_mapper.Map<Location>(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location {LocationId}", id);
            return BadRequest(new { message = "Error retrieving location", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new location
    /// </summary>
    [HttpPost("location")]
    public async Task<ActionResult<Location>> CreateLocation([FromBody] Location dto)
    {
        try
        {
            var location = _mapper.Map<DbLocation>(dto);
            location.CreatedAt = DateTime.UtcNow;
            location.ModifiedAt = DateTime.UtcNow;

            _dbContext.Locations.Add(location);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocation), new { id = location.LocationId }, _mapper.Map<Location>(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating location");
            return BadRequest(new { message = "Error creating location", error = ex.Message });
        }
    }

    /// <summary>
    /// Update location
    /// </summary>
    [HttpPut("location/{id:long}")]
    public async Task<ActionResult<Location>> UpdateLocation(long id, [FromBody] Location dto)
    {
        try
        {
            var location = await _dbContext.Locations.FindAsync(id);
            if (location == null)
                return NotFound();

            _mapper.Map(dto, location);
            location.ModifiedAt = DateTime.UtcNow;

            _dbContext.Locations.Update(location);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Location>(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location {LocationId}", id);
            return BadRequest(new { message = "Error updating location", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete location
    /// </summary>
    [HttpDelete("location/{id:long}")]
    public async Task<IActionResult> DeleteLocation(long id)
    {
        try
        {
            var location = await _dbContext.Locations.FindAsync(id);
            if (location == null)
                return NotFound();

            _dbContext.Locations.Remove(location);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting location {LocationId}", id);
            return BadRequest(new { message = "Error deleting location", error = ex.Message });
        }
    }

    #endregion
}

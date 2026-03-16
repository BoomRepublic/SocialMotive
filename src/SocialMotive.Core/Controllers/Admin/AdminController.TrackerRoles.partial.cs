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
    #region TrackerRoles

    /// <summary>
    /// Get list of tracker roles
    /// </summary>
    [HttpGet("tracker-roles")]
    public async Task<ActionResult<List<TrackerRole>>> GetTrackerRoles()
    {
        try
        {
            var roles = await _dbContext.TrackerRoles
                .OrderBy(r => r.Name)
                .ProjectTo<TrackerRole>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tracker roles");
            return BadRequest(new { message = "Error retrieving tracker roles", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single tracker role by ID
    /// </summary>
    [HttpGet("tracker-role/{id:int}")]
    public async Task<ActionResult<TrackerRole>> GetTrackerRole(int id)
    {
        try
        {
            var role = await _dbContext.TrackerRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            return Ok(_mapper.Map<TrackerRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tracker role {TrackerRoleId}", id);
            return BadRequest(new { message = "Error retrieving tracker role", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new tracker role
    /// </summary>
    [HttpPost("tracker-role")]
    public async Task<ActionResult<TrackerRole>> CreateTrackerRole([FromBody] TrackerRole dto)
    {
        try
        {
            var role = _mapper.Map<DbTrackerRole>(dto);
            role.CreatedAt = DateTime.UtcNow;

            _dbContext.TrackerRoles.Add(role);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrackerRole), new { id = role.TrackerRoleId }, _mapper.Map<TrackerRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tracker role");
            return BadRequest(new { message = "Error creating tracker role", error = ex.Message });
        }
    }

    /// <summary>
    /// Update tracker role
    /// </summary>
    [HttpPut("tracker-role/{id:int}")]
    public async Task<ActionResult<TrackerRole>> UpdateTrackerRole(int id, [FromBody] TrackerRole dto)
    {
        try
        {
            var role = await _dbContext.TrackerRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            _mapper.Map(dto, role);

            _dbContext.TrackerRoles.Update(role);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<TrackerRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tracker role {TrackerRoleId}", id);
            return BadRequest(new { message = "Error updating tracker role", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete tracker role
    /// </summary>
    [HttpDelete("tracker-role/{id:int}")]
    public async Task<IActionResult> DeleteTrackerRole(int id)
    {
        try
        {
            var role = await _dbContext.TrackerRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            _dbContext.TrackerRoles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tracker role {TrackerRoleId}", id);
            return BadRequest(new { message = "Error deleting tracker role", error = ex.Message });
        }
    }

    #endregion
}

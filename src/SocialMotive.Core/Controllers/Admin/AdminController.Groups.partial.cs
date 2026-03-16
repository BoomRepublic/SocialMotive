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
    #region Groups

    /// <summary>
    /// Get list of groups
    /// </summary>
    [HttpGet("groups")]
    public async Task<ActionResult<List<Group>>> GetGroups()
    {
        try
        {
            var groups = await _dbContext.Groups
                .OrderBy(g => g.Name)
                .ProjectTo<Group>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(groups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups");
            return BadRequest(new { message = "Error retrieving groups", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single group by ID
    /// </summary>
    [HttpGet("group/{id:int}")]
    public async Task<ActionResult<Group>> GetGroup(int id)
    {
        try
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            return Ok(_mapper.Map<Group>(group));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group {GroupId}", id);
            return BadRequest(new { message = "Error retrieving group", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new group
    /// </summary>
    [HttpPost("group")]
    public async Task<ActionResult<Group>> CreateGroup([FromBody] Group dto)
    {
        try
        {
            var group = _mapper.Map<DbGroup>(dto);
            group.CreatedAt = DateTime.UtcNow;
            group.ModifiedAt = DateTime.UtcNow;

            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = group.GroupId }, _mapper.Map<Group>(group));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return BadRequest(new { message = "Error creating group", error = ex.Message });
        }
    }

    /// <summary>
    /// Update group
    /// </summary>
    [HttpPut("group/{id:int}")]
    public async Task<ActionResult<Group>> UpdateGroup(int id, [FromBody] Group dto)
    {
        try
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            _mapper.Map(dto, group);
            group.ModifiedAt = DateTime.UtcNow;

            _dbContext.Groups.Update(group);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Group>(group));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId}", id);
            return BadRequest(new { message = "Error updating group", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete group
    /// </summary>
    [HttpDelete("group/{id:int}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        try
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {GroupId}", id);
            return BadRequest(new { message = "Error deleting group", error = ex.Message });
        }
    }

    #endregion
}

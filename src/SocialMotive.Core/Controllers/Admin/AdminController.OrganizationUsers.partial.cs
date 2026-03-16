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
    #region OrganizationUsers

    /// <summary>
    /// Get list of organization users
    /// </summary>
    [HttpGet("organization-users")]
    public async Task<ActionResult<List<OrganizationUser>>> GetOrganizationUsers()
    {
        try
        {
            var users = await _dbContext.OrganizationUsers
                .OrderBy(ou => ou.OrganizationUserId)
                .ProjectTo<OrganizationUser>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization users");
            return BadRequest(new { message = "Error retrieving organization users", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single organization user by ID
    /// </summary>
    [HttpGet("organization-user/{id:int}")]
    public async Task<ActionResult<OrganizationUser>> GetOrganizationUser(int id)
    {
        try
        {
            var user = await _dbContext.OrganizationUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<OrganizationUser>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization user {OrganizationUserId}", id);
            return BadRequest(new { message = "Error retrieving organization user", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new organization user
    /// </summary>
    [HttpPost("organization-user")]
    public async Task<ActionResult<OrganizationUser>> CreateOrganizationUser([FromBody] OrganizationUser dto)
    {
        try
        {
            var user = _mapper.Map<DbOrganizationUser>(dto);
            user.AssingedAt = DateTime.UtcNow;

            _dbContext.OrganizationUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrganizationUser), new { id = user.OrganizationUserId }, _mapper.Map<OrganizationUser>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating organization user");
            return BadRequest(new { message = "Error creating organization user", error = ex.Message });
        }
    }

    /// <summary>
    /// Update organization user
    /// </summary>
    [HttpPut("organization-user/{id:int}")]
    public async Task<ActionResult<OrganizationUser>> UpdateOrganizationUser(int id, [FromBody] OrganizationUser dto)
    {
        try
        {
            var user = await _dbContext.OrganizationUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            _mapper.Map(dto, user);

            _dbContext.OrganizationUsers.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<OrganizationUser>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating organization user {OrganizationUserId}", id);
            return BadRequest(new { message = "Error updating organization user", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete organization user
    /// </summary>
    [HttpDelete("organization-user/{id:int}")]
    public async Task<IActionResult> DeleteOrganizationUser(int id)
    {
        try
        {
            var user = await _dbContext.OrganizationUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            _dbContext.OrganizationUsers.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting organization user {OrganizationUserId}", id);
            return BadRequest(new { message = "Error deleting organization user", error = ex.Message });
        }
    }

    #endregion
}

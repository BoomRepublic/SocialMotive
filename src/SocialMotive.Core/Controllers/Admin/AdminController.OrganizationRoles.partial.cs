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
    #region OrganizationRoles

    /// <summary>
    /// Get list of organization roles
    /// </summary>
    [HttpGet("organization-roles")]
    public async Task<ActionResult<List<OrganizationRole>>> GetOrganizationRoles()
    {
        try
        {
            var roles = await _dbContext.OrganizationRoles
                .OrderBy(r => r.Name)
                .ProjectTo<OrganizationRole>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization roles");
            return BadRequest(new { message = "Error retrieving organization roles", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single organization role by ID
    /// </summary>
    [HttpGet("organization-role/{id:int}")]
    public async Task<ActionResult<OrganizationRole>> GetOrganizationRole(int id)
    {
        try
        {
            var role = await _dbContext.OrganizationRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            return Ok(_mapper.Map<OrganizationRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization role {OrganizationRoleId}", id);
            return BadRequest(new { message = "Error retrieving organization role", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new organization role
    /// </summary>
    [HttpPost("organization-role")]
    public async Task<ActionResult<OrganizationRole>> CreateOrganizationRole([FromBody] OrganizationRole dto)
    {
        try
        {
            var role = _mapper.Map<DbOrganizationRole>(dto);

            _dbContext.OrganizationRoles.Add(role);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrganizationRole), new { id = role.OrganizationRoleId }, _mapper.Map<OrganizationRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating organization role");
            return BadRequest(new { message = "Error creating organization role", error = ex.Message });
        }
    }

    /// <summary>
    /// Update organization role
    /// </summary>
    [HttpPut("organization-role/{id:int}")]
    public async Task<ActionResult<OrganizationRole>> UpdateOrganizationRole(int id, [FromBody] OrganizationRole dto)
    {
        try
        {
            var role = await _dbContext.OrganizationRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            _mapper.Map(dto, role);

            _dbContext.OrganizationRoles.Update(role);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<OrganizationRole>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating organization role {OrganizationRoleId}", id);
            return BadRequest(new { message = "Error updating organization role", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete organization role
    /// </summary>
    [HttpDelete("organization-role/{id:int}")]
    public async Task<IActionResult> DeleteOrganizationRole(int id)
    {
        try
        {
            var role = await _dbContext.OrganizationRoles.FindAsync(id);
            if (role == null)
                return NotFound();

            _dbContext.OrganizationRoles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting organization role {OrganizationRoleId}", id);
            return BadRequest(new { message = "Error deleting organization role", error = ex.Message });
        }
    }

    #endregion
}

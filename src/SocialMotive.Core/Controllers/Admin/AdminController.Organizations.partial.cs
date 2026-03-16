using System.Security.Claims;
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
    #region Organizations

    /// <summary>
    /// Get list of organizations
    /// </summary>
    [HttpGet("organizations")]
    public async Task<ActionResult<List<Organization>>> GetOrganizations()
    {
        try
        {
            var organizations = await _dbContext.Organizations
                .OrderBy(o => o.Name)
                .ProjectTo<Organization>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(organizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organizations");
            return BadRequest(new { message = "Error retrieving organizations", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single organization by ID
    /// </summary>
    [HttpGet("organization/{id:int}")]
    public async Task<ActionResult<Organization>> GetOrganization(int id)
    {
        try
        {
            var organization = await _dbContext.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound();

            return Ok(_mapper.Map<Organization>(organization));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization {OrganizationId}", id);
            return BadRequest(new { message = "Error retrieving organization", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new organization
    /// </summary>
    [HttpPost("organization")]
    public async Task<ActionResult<Organization>> CreateOrganization([FromBody] Organization dto)
    {
        try
        {
            var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;

            var organization = _mapper.Map<DbOrganization>(dto);
            organization.OwnedBy = userId;
            organization.CreatedBy = userId;
            organization.ModifiedBy = userId;
            organization.CreatedAt = DateTime.UtcNow;
            organization.ModifiedAt = DateTime.UtcNow;

            _dbContext.Organizations.Add(organization);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrganization), new { id = organization.OrganizationId }, _mapper.Map<Organization>(organization));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating organization");
            return BadRequest(new { message = "Error creating organization", error = ex.Message });
        }
    }

    /// <summary>
    /// Update organization
    /// </summary>
    [HttpPut("organization/{id:int}")]
    public async Task<ActionResult<Organization>> UpdateOrganization(int id, [FromBody] Organization dto)
    {
        try
        {
            var organization = await _dbContext.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound();

            var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;

            _mapper.Map(dto, organization);
            organization.ModifiedBy = userId;
            organization.ModifiedAt = DateTime.UtcNow;

            _dbContext.Organizations.Update(organization);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Organization>(organization));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating organization {OrganizationId}", id);
            return BadRequest(new { message = "Error updating organization", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete organization
    /// </summary>
    [HttpDelete("organization/{id:int}")]
    public async Task<IActionResult> DeleteOrganization(int id)
    {
        try
        {
            var organization = await _dbContext.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound();

            _dbContext.Organizations.Remove(organization);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting organization {OrganizationId}", id);
            return BadRequest(new { message = "Error deleting organization", error = ex.Message });
        }
    }

    #endregion
}

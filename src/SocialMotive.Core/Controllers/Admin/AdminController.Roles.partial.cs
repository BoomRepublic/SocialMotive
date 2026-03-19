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
    #region Roles

    /// <summary>
    /// Get list of all roles
    /// </summary>
    [HttpGet("roles")]
    public async Task<ActionResult<List<Role>>> GetRoles()
    {
        try
        {
            var roles = await _dbContext.Roles
                .ProjectTo<Role>(_mapper.ConfigurationProvider)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            return BadRequest(new { message = "Error retrieving roles", error = ex.Message });
        }
    }

    /// <summary>
    /// Get a single role by ID
    /// </summary>
    [HttpGet("role/{id:int}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        try
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            return Ok(_mapper.Map<Role>(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId}", id);
            return BadRequest(new { message = "Error retrieving role", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost("role")]
    public async Task<ActionResult<Role>> CreateRole([FromBody] Role dto)
    {
        try
        {
            var entity = _mapper.Map<DbRole>(dto);
            _dbContext.Roles.Add(entity);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = entity.RoleId }, _mapper.Map<Role>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return BadRequest(new { message = "Error creating role", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    [HttpPut("role/{id:int}")]
    public async Task<ActionResult<Role>> UpdateRole(int id, [FromBody] Role dto)
    {
        try
        {
            var entity = await _dbContext.Roles.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(dto, entity);
            _dbContext.Roles.Update(entity);
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Role>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return BadRequest(new { message = "Error updating role", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    [HttpDelete("role/{id:int}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        try
        {
            var entity = await _dbContext.Roles.FindAsync(id);
            if (entity == null)
                return NotFound();

            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return BadRequest(new { message = "Error deleting role", error = ex.Message });
        }
    }

    #endregion

    #region UserRoles

    /// <summary>
    /// Get roles assigned to a specific user
    /// </summary>
    [HttpGet("user/{userId:int}/roles")]
    public async Task<ActionResult<List<UserRole>>> GetUserRoles(int userId)
    {
        try
        {
            var userRoles = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ProjectTo<UserRole>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return BadRequest(new { message = "Error retrieving user roles", error = ex.Message });
        }
    }

    /// <summary>
    /// Replace all role assignments for a user with the provided role IDs
    /// </summary>
    [HttpPut("user/{userId:int}/roles")]
    public async Task<ActionResult<List<UserRole>>> UpdateUserRoles(int userId, [FromBody] List<int> roleIds)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Remove existing roles
            var existingRoles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            _dbContext.UserRoles.RemoveRange(existingRoles);

            // Add new roles
            foreach (var roleId in roleIds)
            {
                _dbContext.UserRoles.Add(new DbUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }

            await _dbContext.SaveChangesAsync();

            // Return updated list
            var updatedRoles = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ProjectTo<UserRole>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(updatedRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating roles for user {UserId}", userId);
            return BadRequest(new { message = "Error updating user roles", error = ex.Message });
        }
    }

    #endregion
}

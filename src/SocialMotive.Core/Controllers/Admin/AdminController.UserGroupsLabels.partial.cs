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
    #region UserGroups

    /// <summary>
    /// Get groups assigned to a specific user
    /// </summary>
    [HttpGet("user/{userId:int}/groups")]
    public async Task<ActionResult<List<UserGroup>>> GetUserGroups(int userId)
    {
        try
        {
            var userGroups = await _dbContext.UserGroups
                .Include(ug => ug.Group)
                .Where(ug => ug.UserId == userId)
                .ProjectTo<UserGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(userGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups for user {UserId}", userId);
            return BadRequest(new { message = "Error retrieving user groups", error = ex.Message });
        }
    }

    /// <summary>
    /// Replace all group assignments for a user with the provided group IDs
    /// </summary>
    [HttpPut("user/{userId:int}/groups")]
    public async Task<ActionResult<List<UserGroup>>> UpdateUserGroups(int userId, [FromBody] List<int> groupIds)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Remove existing groups
            var existingGroups = await _dbContext.UserGroups
                .Where(ug => ug.UserId == userId)
                .ToListAsync();
            _dbContext.UserGroups.RemoveRange(existingGroups);

            // Add new groups
            foreach (var groupId in groupIds)
            {
                _dbContext.UserGroups.Add(new DbUserGroup
                {
                    UserId = userId,
                    GroupId = groupId
                });
            }

            await _dbContext.SaveChangesAsync();

            // Return updated list
            var updatedGroups = await _dbContext.UserGroups
                .Include(ug => ug.Group)
                .Where(ug => ug.UserId == userId)
                .ProjectTo<UserGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(updatedGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating groups for user {UserId}", userId);
            return BadRequest(new { message = "Error updating user groups", error = ex.Message });
        }
    }

    #endregion

    #region UserLabels

    /// <summary>
    /// Get labels assigned to a specific user
    /// </summary>
    [HttpGet("user/{userId:int}/labels")]
    public async Task<ActionResult<List<UserLabel>>> GetUserLabels(int userId)
    {
        try
        {
            var userLabels = await _dbContext.UserLabels
                .Include(ul => ul.Label)
                .Where(ul => ul.UserId == userId)
                .ProjectTo<UserLabel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(userLabels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting labels for user {UserId}", userId);
            return BadRequest(new { message = "Error retrieving user labels", error = ex.Message });
        }
    }

    /// <summary>
    /// Replace all label assignments for a user with the provided label IDs
    /// </summary>
    [HttpPut("user/{userId:int}/labels")]
    public async Task<ActionResult<List<UserLabel>>> UpdateUserLabels(int userId, [FromBody] List<int> labelIds)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Remove existing labels
            var existingLabels = await _dbContext.UserLabels
                .Where(ul => ul.UserId == userId)
                .ToListAsync();
            _dbContext.UserLabels.RemoveRange(existingLabels);

            // Add new labels
            foreach (var labelId in labelIds)
            {
                _dbContext.UserLabels.Add(new DbUserLabel
                {
                    UserId = userId,
                    LabelId = labelId
                });
            }

            await _dbContext.SaveChangesAsync();

            // Return updated list
            var updatedLabels = await _dbContext.UserLabels
                .Include(ul => ul.Label)
                .Where(ul => ul.UserId == userId)
                .ProjectTo<UserLabel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(updatedLabels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating labels for user {UserId}", userId);
            return BadRequest(new { message = "Error updating user labels", error = ex.Message });
        }
    }

    #endregion
}

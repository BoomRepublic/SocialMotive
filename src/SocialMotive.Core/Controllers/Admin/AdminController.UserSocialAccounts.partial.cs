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
    #region UserSocialAccounts

    /// <summary>
    /// Get list of user social accounts
    /// </summary>
    [HttpGet("user-social-accounts")]
    public async Task<ActionResult<List<UserSocialAccount>>> GetUserSocialAccounts()
    {
        try
        {
            var accounts = await _dbContext.UserSocialAccounts
                .ProjectTo<UserSocialAccount>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user social accounts");
            return BadRequest(new { message = "Error retrieving user social accounts", error = ex.Message });
        }
    }

    /// <summary>
    /// Get user social accounts by user ID
    /// </summary>
    [HttpGet("user/{userId:int}/social-accounts")]
    public async Task<ActionResult<List<UserSocialAccount>>> GetUserSocialAccountsByUser(int userId)
    {
        try
        {
            var accounts = await _dbContext.UserSocialAccounts
                .Where(a => a.UserId == userId)
                .ProjectTo<UserSocialAccount>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social accounts for user {UserId}", userId);
            return BadRequest(new { message = "Error retrieving user social accounts", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single user social account by ID
    /// </summary>
    [HttpGet("user-social-account/{id:int}")]
    public async Task<ActionResult<UserSocialAccount>> GetUserSocialAccount(int id)
    {
        try
        {
            var account = await _dbContext.UserSocialAccounts.FindAsync(id);
            if (account == null)
                return NotFound();

            return Ok(_mapper.Map<UserSocialAccount>(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user social account {UserSocialAccountId}", id);
            return BadRequest(new { message = "Error retrieving user social account", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new user social account
    /// </summary>
    [HttpPost("user-social-account")]
    public async Task<ActionResult<UserSocialAccount>> CreateUserSocialAccount([FromBody] UserSocialAccount dto)
    {
        try
        {
            var account = _mapper.Map<DbUserSocialAccount>(dto);
            account.Created = DateTime.UtcNow;
            account.Modified = DateTime.UtcNow;

            _dbContext.UserSocialAccounts.Add(account);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserSocialAccount), new { id = account.UserSocialAccountId }, _mapper.Map<UserSocialAccount>(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user social account");
            return BadRequest(new { message = "Error creating user social account", error = ex.Message });
        }
    }

    /// <summary>
    /// Update user social account
    /// </summary>
    [HttpPut("user-social-account/{id:int}")]
    public async Task<ActionResult<UserSocialAccount>> UpdateUserSocialAccount(int id, [FromBody] UserSocialAccount dto)
    {
        try
        {
            var account = await _dbContext.UserSocialAccounts.FindAsync(id);
            if (account == null)
                return NotFound();

            _mapper.Map(dto, account);
            account.Modified = DateTime.UtcNow;

            _dbContext.UserSocialAccounts.Update(account);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<UserSocialAccount>(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user social account {UserSocialAccountId}", id);
            return BadRequest(new { message = "Error updating user social account", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete user social account
    /// </summary>
    [HttpDelete("user-social-account/{id:int}")]
    public async Task<IActionResult> DeleteUserSocialAccount(int id)
    {
        try
        {
            var account = await _dbContext.UserSocialAccounts.FindAsync(id);
            if (account == null)
                return NotFound();

            _dbContext.UserSocialAccounts.Remove(account);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user social account {UserSocialAccountId}", id);
            return BadRequest(new { message = "Error deleting user social account", error = ex.Message });
        }
    }

    #endregion
}

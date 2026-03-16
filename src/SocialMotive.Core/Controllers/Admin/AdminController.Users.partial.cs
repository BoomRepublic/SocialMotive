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
    #region Users

    /// <summary>
    /// Get list of users
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        try
        {
            var users = await _dbContext.Users
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return BadRequest(new { message = "Error retrieving users", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single user by ID
    /// </summary>
    [HttpGet("user/{id:int}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<User>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return BadRequest(new { message = "Error retrieving user", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost("user")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User User)
    {
        try
        {
            var user = _mapper.Map<DbUser>(User);
            user.PasswordHash = "default_hash"; // TODO: Implement proper password hashing
            user.Created = DateTime.UtcNow;
            user.Modified = DateTime.UtcNow;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, _mapper.Map<User>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(new { message = "Error creating user", error = ex.Message });
        }
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("user/{id:int}")]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User User)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _mapper.Map(User, user);
            user.Modified = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<User>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return BadRequest(new { message = "Error updating user", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("user/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return BadRequest(new { message = "Error deleting user", error = ex.Message });
        }
    }

    #endregion
}

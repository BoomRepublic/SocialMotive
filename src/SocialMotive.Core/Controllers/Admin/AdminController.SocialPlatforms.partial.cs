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
    #region SocialPlatforms

    /// <summary>
    /// Get list of social platforms
    /// </summary>
    [HttpGet("social-platforms")]
    public async Task<ActionResult<List<SocialPlatform>>> GetSocialPlatforms()
    {
        try
        {
            var platforms = await _dbContext.SocialPlatforms
                .ProjectTo<SocialPlatform>(_mapper.ConfigurationProvider)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Ok(platforms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social platforms");
            return BadRequest(new { message = "Error retrieving social platforms", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single social platform by ID
    /// </summary>
    [HttpGet("social-platform/{id:int}")]
    public async Task<ActionResult<SocialPlatform>> GetSocialPlatform(int id)
    {
        try
        {
            var platform = await _dbContext.SocialPlatforms.FindAsync(id);
            if (platform == null)
                return NotFound();

            return Ok(_mapper.Map<SocialPlatform>(platform));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social platform {SocialPlatformId}", id);
            return BadRequest(new { message = "Error retrieving social platform", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new social platform
    /// </summary>
    [HttpPost("social-platform")]
    public async Task<ActionResult<SocialPlatform>> CreateSocialPlatform([FromBody] SocialPlatform dto)
    {
        try
        {
            var platform = _mapper.Map<DbSocialPlatform>(dto);

            _dbContext.SocialPlatforms.Add(platform);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSocialPlatform), new { id = platform.SocialPlatformId }, _mapper.Map<SocialPlatform>(platform));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating social platform");
            return BadRequest(new { message = "Error creating social platform", error = ex.Message });
        }
    }

    /// <summary>
    /// Update social platform
    /// </summary>
    [HttpPut("social-platform/{id:int}")]
    public async Task<ActionResult<SocialPlatform>> UpdateSocialPlatform(int id, [FromBody] SocialPlatform dto)
    {
        try
        {
            var platform = await _dbContext.SocialPlatforms.FindAsync(id);
            if (platform == null)
                return NotFound();

            _mapper.Map(dto, platform);

            _dbContext.SocialPlatforms.Update(platform);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<SocialPlatform>(platform));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating social platform {SocialPlatformId}", id);
            return BadRequest(new { message = "Error updating social platform", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete social platform
    /// </summary>
    [HttpDelete("social-platform/{id:int}")]
    public async Task<IActionResult> DeleteSocialPlatform(int id)
    {
        try
        {
            var platform = await _dbContext.SocialPlatforms.FindAsync(id);
            if (platform == null)
                return NotFound();

            _dbContext.SocialPlatforms.Remove(platform);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting social platform {SocialPlatformId}", id);
            return BadRequest(new { message = "Error deleting social platform", error = ex.Message });
        }
    }

    #endregion
}

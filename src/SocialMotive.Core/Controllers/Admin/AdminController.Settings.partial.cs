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
    #region Settings

    /// <summary>
    /// Get list of settings
    /// </summary>
    [HttpGet("settings")]
    public async Task<ActionResult<List<Setting>>> GetSettings()
    {
        try
        {
            var settings = await _dbContext.Settings
                .OrderBy(s => s.SettingKey)
                .ProjectTo<Setting>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting settings");
            return BadRequest(new { message = "Error retrieving settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single setting by ID
    /// </summary>
    [HttpGet("setting/{id:int}")]
    public async Task<ActionResult<Setting>> GetSetting(int id)
    {
        try
        {
            var setting = await _dbContext.Settings.FindAsync(id);
            if (setting == null)
                return NotFound();

            return Ok(_mapper.Map<Setting>(setting));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting setting {SettingId}", id);
            return BadRequest(new { message = "Error retrieving setting", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new setting
    /// </summary>
    [HttpPost("setting")]
    public async Task<ActionResult<Setting>> CreateSetting([FromBody] Setting dto)
    {
        try
        {
            var setting = _mapper.Map<DbSetting>(dto);

            _dbContext.Settings.Add(setting);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSetting), new { id = setting.SettingId }, _mapper.Map<Setting>(setting));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating setting");
            return BadRequest(new { message = "Error creating setting", error = ex.Message });
        }
    }

    /// <summary>
    /// Update setting
    /// </summary>
    [HttpPut("setting/{id:int}")]
    public async Task<ActionResult<Setting>> UpdateSetting(int id, [FromBody] Setting dto)
    {
        try
        {
            var setting = await _dbContext.Settings.FindAsync(id);
            if (setting == null)
                return NotFound();

            _mapper.Map(dto, setting);

            _dbContext.Settings.Update(setting);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Setting>(setting));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating setting {SettingId}", id);
            return BadRequest(new { message = "Error updating setting", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete setting
    /// </summary>
    [HttpDelete("setting/{id:int}")]
    public async Task<IActionResult> DeleteSetting(int id)
    {
        try
        {
            var setting = await _dbContext.Settings.FindAsync(id);
            if (setting == null)
                return NotFound();

            _dbContext.Settings.Remove(setting);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting setting {SettingId}", id);
            return BadRequest(new { message = "Error deleting setting", error = ex.Message });
        }
    }

    #endregion
}

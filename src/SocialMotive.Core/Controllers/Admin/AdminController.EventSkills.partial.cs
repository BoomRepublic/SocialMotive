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
    #region EventSkills

    /// <summary>
    /// Get list of event skills
    /// </summary>
    [HttpGet("event-skills")]
    public async Task<ActionResult<List<EventSkill>>> GetEventSkills()
    {
        try
        {
            var eventSkills = await _dbContext.EventSkills
                .ProjectTo<EventSkill>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(eventSkills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event skills");
            return BadRequest(new { message = "Error retrieving event skills", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single event skill by ID
    /// </summary>
    [HttpGet("event-skill/{id:int}")]
    public async Task<ActionResult<EventSkill>> GetEventSkill(int id)
    {
        try
        {
            var eventSkill = await _dbContext.EventSkills.FindAsync(id);
            if (eventSkill == null)
                return NotFound();

            return Ok(_mapper.Map<EventSkill>(eventSkill));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event skill {EventSkillId}", id);
            return BadRequest(new { message = "Error retrieving event skill", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new event skill
    /// </summary>
    [HttpPost("event-skill")]
    public async Task<ActionResult<EventSkill>> CreateEventSkill([FromBody] EventSkill eventSkillDto)
    {
        try
        {
            var eventSkill = _mapper.Map<DbEventSkill>(eventSkillDto);

            _dbContext.EventSkills.Add(eventSkill);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventSkill), new { id = eventSkill.EventSkillId }, _mapper.Map<EventSkill>(eventSkill));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event skill");
            return BadRequest(new { message = "Error creating event skill", error = ex.Message });
        }
    }

    /// <summary>
    /// Update event skill
    /// </summary>
    [HttpPut("event-skill/{id:int}")]
    public async Task<ActionResult<EventSkill>> UpdateEventSkill(int id, [FromBody] EventSkill eventSkillDto)
    {
        try
        {
            var eventSkill = await _dbContext.EventSkills.FindAsync(id);
            if (eventSkill == null)
                return NotFound();

            _mapper.Map(eventSkillDto, eventSkill);

            _dbContext.EventSkills.Update(eventSkill);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<EventSkill>(eventSkill));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event skill {EventSkillId}", id);
            return BadRequest(new { message = "Error updating event skill", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete event skill
    /// </summary>
    [HttpDelete("event-skill/{id:int}")]
    public async Task<IActionResult> DeleteEventSkill(int id)
    {
        try
        {
            var eventSkill = await _dbContext.EventSkills.FindAsync(id);
            if (eventSkill == null)
                return NotFound();

            _dbContext.EventSkills.Remove(eventSkill);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event skill {EventSkillId}", id);
            return BadRequest(new { message = "Error deleting event skill", error = ex.Message });
        }
    }

    #endregion
}

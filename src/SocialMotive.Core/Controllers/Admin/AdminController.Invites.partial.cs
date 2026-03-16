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
    #region Invites

    /// <summary>
    /// Get list of invites
    /// </summary>
    [HttpGet("invites")]
    public async Task<ActionResult<List<Invite>>> GetInvites()
    {
        try
        {
            var invites = await _dbContext.Invites
                .ProjectTo<Invite>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(invites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invites");
            return BadRequest(new { message = "Error retrieving invites", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single invite by ID
    /// </summary>
    [HttpGet("invite/{id:int}")]
    public async Task<ActionResult<Invite>> GetInvite(int id)
    {
        try
        {
            var invite = await _dbContext.Invites.FindAsync(id);
            if (invite == null)
                return NotFound();

            return Ok(_mapper.Map<Invite>(invite));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invite {InviteId}", id);
            return BadRequest(new { message = "Error retrieving invite", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new invite
    /// </summary>
    [HttpPost("invite")]
    public async Task<ActionResult<Invite>> CreateInvite([FromBody] Invite dto)
    {
        try
        {
            var invite = _mapper.Map<DbInvite>(dto);
            invite.CreatedAt = DateTime.UtcNow;

            _dbContext.Invites.Add(invite);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvite), new { id = invite.InviteId }, _mapper.Map<Invite>(invite));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invite");
            return BadRequest(new { message = "Error creating invite", error = ex.Message });
        }
    }

    /// <summary>
    /// Update invite
    /// </summary>
    [HttpPut("invite/{id:int}")]
    public async Task<ActionResult<Invite>> UpdateInvite(int id, [FromBody] Invite dto)
    {
        try
        {
            var invite = await _dbContext.Invites.FindAsync(id);
            if (invite == null)
                return NotFound();

            _mapper.Map(dto, invite);

            _dbContext.Invites.Update(invite);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Invite>(invite));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invite {InviteId}", id);
            return BadRequest(new { message = "Error updating invite", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete invite
    /// </summary>
    [HttpDelete("invite/{id:int}")]
    public async Task<IActionResult> DeleteInvite(int id)
    {
        try
        {
            var invite = await _dbContext.Invites.FindAsync(id);
            if (invite == null)
                return NotFound();

            _dbContext.Invites.Remove(invite);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invite {InviteId}", id);
            return BadRequest(new { message = "Error deleting invite", error = ex.Message });
        }
    }

    #endregion
}

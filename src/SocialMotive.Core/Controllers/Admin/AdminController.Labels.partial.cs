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
    #region Labels

    /// <summary>
    /// Get list of labels
    /// </summary>
    [HttpGet("labels")]
    public async Task<ActionResult<List<Label>>> GetLabels()
    {
        try
        {
            var labels = await _dbContext.Labels
                .ProjectTo<Label>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting labels");
            return BadRequest(new { message = "Error retrieving labels", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single label by ID
    /// </summary>
    [HttpGet("label/{id:int}")]
    public async Task<ActionResult<Label>> GetLabel(int id)
    {
        try
        {
            var label = await _dbContext.Labels.FindAsync(id);
            if (label == null)
                return NotFound();

            return Ok(_mapper.Map<Label>(label));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting label {LabelId}", id);
            return BadRequest(new { message = "Error retrieving label", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new label
    /// </summary>
    [HttpPost("label")]
    public async Task<ActionResult<Label>> CreateLabel([FromBody] Label Label)
    {
        try
        {
            var label = _mapper.Map<DbLabel>(Label);

            _dbContext.Labels.Add(label);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLabel), new { id = label.LabelId }, _mapper.Map<Label>(label));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating label");
            return BadRequest(new { message = "Error creating label", error = ex.Message });
        }
    }

    /// <summary>
    /// Update label
    /// </summary>
    [HttpPut("label/{id:int}")]
    public async Task<ActionResult<Label>> UpdateLabel(int id, [FromBody] Label Label)
    {
        try
        {
            var label = await _dbContext.Labels.FindAsync(id);
            if (label == null)
                return NotFound();

            _mapper.Map(Label, label);

            _dbContext.Labels.Update(label);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Label>(label));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating label {LabelId}", id);
            return BadRequest(new { message = "Error updating label", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete label
    /// </summary>
    [HttpDelete("label/{id:int}")]
    public async Task<IActionResult> DeleteLabel(int id)
    {
        try
        {
            var label = await _dbContext.Labels.FindAsync(id);
            if (label == null)
                return NotFound();

            _dbContext.Labels.Remove(label);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId}", id);
            return BadRequest(new { message = "Error deleting label", error = ex.Message });
        }
    }

    #endregion
}

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
    #region Cities

    /// <summary>
    /// Get list of cities
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<List<City>>> GetCities()
    {
        try
        {
            var cities = await _dbContext.Cities
                .OrderBy(c => c.Name)
                .ProjectTo<City>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cities");
            return BadRequest(new { message = "Error retrieving cities", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single city by ID
    /// </summary>
    [HttpGet("city/{id:int}")]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        try
        {
            var city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            return Ok(_mapper.Map<City>(city));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting city {CityId}", id);
            return BadRequest(new { message = "Error retrieving city", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new city
    /// </summary>
    [HttpPost("city")]
    public async Task<ActionResult<City>> CreateCity([FromBody] City dto)
    {
        try
        {
            var city = _mapper.Map<DbCity>(dto);

            _dbContext.Cities.Add(city);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCity), new { id = city.CityId }, _mapper.Map<City>(city));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating city");
            return BadRequest(new { message = "Error creating city", error = ex.Message });
        }
    }

    /// <summary>
    /// Update city
    /// </summary>
    [HttpPut("city/{id:int}")]
    public async Task<ActionResult<City>> UpdateCity(int id, [FromBody] City dto)
    {
        try
        {
            var city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            _mapper.Map(dto, city);

            _dbContext.Cities.Update(city);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<City>(city));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating city {CityId}", id);
            return BadRequest(new { message = "Error updating city", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete city
    /// </summary>
    [HttpDelete("city/{id:int}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        try
        {
            var city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            _dbContext.Cities.Remove(city);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting city {CityId}", id);
            return BadRequest(new { message = "Error deleting city", error = ex.Message });
        }
    }

    #endregion
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;
using SocialMotive.Core.Data.Generator;
using SocialMotive.Core.Generator;
using System.Text.Json;
using SkiaSharp;
using SocialMotive.Core.Model.Generator;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Designer API — manages templates, layers, assets, and render jobs for the Designer app.
/// </summary>
[ApiController]
[Route("api/design")]
[Authorize(Roles = AppRoles.Admin)]
public class DesignController : ControllerBase
{
    private readonly ILogger<DesignController> _logger;
    private readonly SocialMotiveDbContext _db;
    private readonly IMapper _mapper;

    public DesignController(ILogger<DesignController> logger, SocialMotiveDbContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    #region Templates

    /// <summary>
    /// Get list of design templates
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<List<TemplateSummary>>> GetTemplates()
    {
        try
        {
            var entities = await _db.Templates.ToListAsync();
            var templates = _mapper.Map<List<TemplateSummary>>(entities);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates");
            return BadRequest(new { message = "Error retrieving templates", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single template by ID including its layers
    /// </summary>
    [HttpGet("templates/{id:int}")]
    public async Task<ActionResult<TemplateDetail>> GetTemplate(int id)
    {
        try
        {
            var template = await _db.Templates
                .FirstOrDefaultAsync(t => t.TemplateId == id);

            if (template == null)
                return NotFound();

            return Ok(_mapper.Map<TemplateDetail>(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {TemplateId}", id);
            return BadRequest(new { message = "Error retrieving template", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new design template
    /// </summary>
    [HttpPost("templates")]
    public async Task<ActionResult<TemplateDetail>> CreateTemplate([FromBody] CreateTemplateRequest request)
    {
        try
        {
            var template = new DbTemplate
            {
                Name = request.Name,
                Description = request.Description,
                Width = request.Width,
                Height = request.Height,
                Tags = request.Tags,
                Category = request.Category,
                IsPublished = false,
                IsTemplate = true,
                TemplateJson = TemplateJsonHelper.Serialize(new TemplateData()),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Templates.Add(template);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplate), new { id = template.TemplateId }, _mapper.Map<TemplateDetail>(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return BadRequest(new { message = "Error creating template", error = ex.Message });
        }
    }

    /// <summary>
    /// Update design template
    /// </summary>
    [HttpPut("templates/{id:int}")]
    public async Task<ActionResult<TemplateDetail>> UpdateTemplate(int id, [FromBody] UpdateTemplateRequest request)
    {
        try
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            if (request.Name != null) template.Name = request.Name;
            if (request.Description != null) template.Description = request.Description;
            if (request.Width.HasValue) template.Width = request.Width.Value;
            if (request.Height.HasValue) template.Height = request.Height.Value;
            if (request.Tags != null) template.Tags = request.Tags;
            if (request.Category != null) template.Category = request.Category;
            if (request.IsPublished.HasValue) template.IsPublished = request.IsPublished.Value;

            template.UpdatedAt = DateTime.UtcNow;

            _db.Templates.Update(template);
            await _db.SaveChangesAsync();

            return Ok(_mapper.Map<TemplateDetail>(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", id);
            return BadRequest(new { message = "Error updating template", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete design template
    /// </summary>
    [HttpDelete("templates/{id:int}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        try
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            _db.Templates.Remove(template);
            await _db.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", id);
            return BadRequest(new { message = "Error deleting template", error = ex.Message });
        }
    }

    #endregion

    #region Layers

    /// <summary>
    /// Update all layers for a template (full replacement)
    /// </summary>
    [HttpPut("templates/{id:int}/layers")]
    public async Task<ActionResult<TemplateDetail>> UpdateTemplateLayers(int id, [FromBody] List<Layer> layers)
    {
        try
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            var data = TemplateJsonHelper.Deserialize(template.TemplateJson);
            data.Layers = layers;
            template.TemplateJson = TemplateJsonHelper.Serialize(data);
            template.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(_mapper.Map<TemplateDetail>(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating layers for template {TemplateId}", id);
            return BadRequest(new { message = "Error updating layers", error = ex.Message });
        }
    }

    #endregion

    #region Assets

    /// <summary>
    /// Get list of assets
    /// </summary>
    [HttpGet("assets")]
    public async Task<ActionResult<List<Asset>>> GetAssets()
    {
        try
        {
            var assets = await _db.Assets
                .Select(a => _mapper.Map<Asset>(a))
                .ToListAsync();

            return Ok(assets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assets");
            return BadRequest(new { message = "Error retrieving assets", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single asset by ID
    /// </summary>
    [HttpGet("assets/{id:int}")]
    public async Task<ActionResult<Asset>> GetAsset(int id)
    {
        try
        {
            var asset = await _db.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();

            return Ok(_mapper.Map<Asset>(asset));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset {AssetId}", id);
            return BadRequest(new { message = "Error retrieving asset", error = ex.Message });
        }
    }

    /// <summary>
    /// Get asset raw PNG bytes — used for canvas image preview
    /// </summary>
    [HttpGet("assets/{id:int}/image")]
    public async Task<IActionResult> GetAssetImage(int id)
    {
        try
        {
            var asset = await _db.Assets.FindAsync(id);
            if (asset == null || asset.ImagePng == null || asset.ImagePng.Length == 0)
                return NotFound();

            return File(asset.ImagePng, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset image {AssetId}", id);
            return BadRequest(new { message = "Error retrieving asset image", error = ex.Message });
        }
    }

    /// <summary>
    /// Upload / create a new asset
    /// </summary>
    [HttpPost("assets")]
    public async Task<ActionResult<Asset>> CreateAsset([FromBody] UploadAssetRequest request)
    {
        try
        {
            var asset = _mapper.Map<DbAsset>(request);
            asset.CreatedAt = DateTime.UtcNow;
            asset.UpdatedAt = DateTime.UtcNow;

            // Extract image metadata from PNG bytes
            if (asset.ImagePng is { Length: > 0 })
            {
                using var bitmap = SKBitmap.Decode(asset.ImagePng);
                if (bitmap != null)
                {
                    var meta = new ImageMetaData
                    {
                        Width = bitmap.Width,
                        Height = bitmap.Height,
                        FileSize = asset.ImagePng.Length,
                        HasAlpha = bitmap.AlphaType != SKAlphaType.Opaque,
                        MimeType = "image/png"
                    };
                    asset.ImageMetaDataJson = JsonSerializer.Serialize(meta);
                }
            }

            _db.Assets.Add(asset);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAsset), new { id = asset.AssetId }, _mapper.Map<Asset>(asset));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return BadRequest(new { message = "Error creating asset", error = ex.Message });
        }
    }

    /// <summary>
    /// Update asset metadata (filename, tags, visibility)
    /// </summary>
    [HttpPut("assets/{id:int}")]
    public async Task<ActionResult<Asset>> UpdateAsset(int id, [FromBody] Asset request)
    {
        try
        {
            var asset = await _db.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();

            asset.FileName = request.FileName;
            asset.Tags = request.Tags;
            asset.IsPublic = request.IsPublic;
            asset.UpdatedAt = DateTime.UtcNow;

            _db.Assets.Update(asset);
            await _db.SaveChangesAsync();

            return Ok(_mapper.Map<Asset>(asset));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset {AssetId}", id);
            return BadRequest(new { message = "Error updating asset", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete asset
    /// </summary>
    [HttpDelete("assets/{id:int}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        try
        {
            var asset = await _db.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();

            _db.Assets.Remove(asset);
            await _db.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset {AssetId}", id);
            return BadRequest(new { message = "Error deleting asset", error = ex.Message });
        }
    }

    #endregion

    #region Render Jobs

    /// <summary>
    /// Get list of render jobs
    /// </summary>
    [HttpGet("renderjobs")]
    public async Task<ActionResult<List<RenderJobStatus>>> GetRenderJobs()
    {
        try
        {
            var jobs = await _db.RenderJobs
                .Select(j => _mapper.Map<RenderJobStatus>(j))
                .ToListAsync();

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting render jobs");
            return BadRequest(new { message = "Error retrieving render jobs", error = ex.Message });
        }
    }

    /// <summary>
    /// Get single render job including result image
    /// </summary>
    [HttpGet("renderjobs/{id:int}")]
    public async Task<ActionResult<RenderPngResponse>> GetRenderJob(int id)
    {
        try
        {
            var job = await _db.RenderJobs.FindAsync(id);
            if (job == null)
                return NotFound();

            return Ok(_mapper.Map<RenderPngResponse>(job));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting render job {RenderJobId}", id);
            return BadRequest(new { message = "Error retrieving render job", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete render job and its result
    /// </summary>
    [HttpDelete("renderjobs/{id:int}")]
    public async Task<IActionResult> DeleteRenderJob(int id)
    {
        try
        {
            var job = await _db.RenderJobs.FindAsync(id);
            if (job == null)
                return NotFound();

            _db.RenderJobs.Remove(job);
            await _db.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting render job {RenderJobId}", id);
            return BadRequest(new { message = "Error deleting render job", error = ex.Message });
        }
    }

    #endregion
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Model.Generator;
using SocialMotive.Core.Data;
using SocialMotive.Core.Data.Generator;
using SocialMotive.Core.Generator;

namespace SocialMotive.Core.Controllers
{
    /// <summary>
    /// Generator Controller for managing templates, assets, layers, and render jobs.
    /// Endpoints: GET, POST (Create), PUT (Update), DELETE
    /// All operations require AssetManager or Admin role.
    /// </summary>
    [ApiController]
    [Route("api/generator")]
    [Authorize(Roles = AppRoles.AssetManager + "," + AppRoles.Admin)]
    public class GeneratorController : ControllerBase
    {
        private readonly ILogger<GeneratorController> _logger;
        private readonly SocialMotiveDbContext _dbContext;
        private readonly IMapper _mapper;

        public GeneratorController(
            ILogger<GeneratorController> logger,
            SocialMotiveDbContext dbContext,
            IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #region Templates

        /// <summary>
        /// Get list of templates
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<List<TemplateSummary>>> GetTemplates()
        {
            try
            {
                var entities = await _dbContext.Templates.ToListAsync();
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
        /// Get single template by ID with layers
        /// </summary>
        [HttpGet("templates/{id:int}")]
        public async Task<ActionResult<TemplateDetail>> GetTemplate(int id)
        {
            try
            {
                var template = await _dbContext.Templates
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
        /// Create new template
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

                _dbContext.Templates.Add(template);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTemplate), new { id = template.TemplateId }, _mapper.Map<TemplateDetail>(template));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template");
                return BadRequest(new { message = "Error creating template", error = ex.Message });
            }
        }

        /// <summary>
        /// Update template
        /// </summary>
        [HttpPut("templates/{id:int}")]
        public async Task<ActionResult<TemplateDetail>> UpdateTemplate(int id, [FromBody] UpdateTemplateRequest request)
        {
            try
            {
                var template = await _dbContext.Templates.FindAsync(id);
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

                _dbContext.Templates.Update(template);
                await _dbContext.SaveChangesAsync();

                return Ok(_mapper.Map<TemplateDetail>(template));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template {TemplateId}", id);
                return BadRequest(new { message = "Error updating template", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete template
        /// </summary>
        [HttpDelete("templates/{id:int}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                var template = await _dbContext.Templates.FindAsync(id);
                if (template == null)
                    return NotFound();

                _dbContext.Templates.Remove(template);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template {TemplateId}", id);
                return BadRequest(new { message = "Error deleting template", error = ex.Message });
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
                var entities = await _dbContext.Assets.ToListAsync();
                var assets = _mapper.Map<List<Asset>>(entities);
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
                var asset = await _dbContext.Assets.FindAsync(id);
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
        /// Upload/create new asset
        /// </summary>
        [HttpPost("assets")]
        public async Task<ActionResult<Asset>> CreateAsset([FromBody] UploadAssetRequest request)
        {
            try
            {
                var asset = _mapper.Map<DbAsset>(request);
                asset.CreatedAt = DateTime.UtcNow;
                asset.UpdatedAt = DateTime.UtcNow;

                _dbContext.Assets.Add(asset);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAsset), new { id = asset.AssetId }, _mapper.Map<Asset>(asset));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                return BadRequest(new { message = "Error creating asset", error = ex.Message });
            }
        }

        /// <summary>
        /// Update asset metadata
        /// </summary>
        [HttpPut("assets/{id:int}")]
        public async Task<ActionResult<Asset>> UpdateAsset(int id, [FromBody] Asset Asset)
        {
            try
            {
                var asset = await _dbContext.Assets.FindAsync(id);
                if (asset == null)
                    return NotFound();

                asset.FileName = Asset.FileName;
                asset.Tags = Asset.Tags;
                asset.IsPublic = Asset.IsPublic;
                asset.UpdatedAt = DateTime.UtcNow;

                _dbContext.Assets.Update(asset);
                await _dbContext.SaveChangesAsync();

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
                var asset = await _dbContext.Assets.FindAsync(id);
                if (asset == null)
                    return NotFound();

                _dbContext.Assets.Remove(asset);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {AssetId}", id);
                return BadRequest(new { message = "Error deleting asset", error = ex.Message });
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
                var template = await _dbContext.Templates.FindAsync(id);
                if (template == null)
                    return NotFound();

                var data = TemplateJsonHelper.Deserialize(template.TemplateJson);
                data.Layers = layers;
                template.TemplateJson = TemplateJsonHelper.Serialize(data);
                template.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return Ok(_mapper.Map<TemplateDetail>(template));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating layers for template {TemplateId}", id);
                return BadRequest(new { message = "Error updating layers", error = ex.Message });
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
                var entities = await _dbContext.RenderJobs.ToListAsync();
                var renderJobs = _mapper.Map<List<RenderJobStatus>>(entities);
                return Ok(renderJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting render jobs");
                return BadRequest(new { message = "Error retrieving render jobs", error = ex.Message });
            }
        }

        /// <summary>
        /// Get single render job by ID
        /// </summary>
        [HttpGet("renderjobs/{id:int}")]
        public async Task<ActionResult<RenderJobStatus>> GetRenderJob(int id)
        {
            try
            {
                var renderJob = await _dbContext.RenderJobs.FindAsync(id);
                if (renderJob == null)
                    return NotFound();

                return Ok(_mapper.Map<RenderJobStatus>(renderJob));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting render job {RenderJobId}", id);
                return BadRequest(new { message = "Error retrieving render job", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete render job
        /// </summary>
        [HttpDelete("renderjobs/{id:int}")]
        public async Task<IActionResult> DeleteRenderJob(int id)
        {
            try
            {
                var renderJob = await _dbContext.RenderJobs.FindAsync(id);
                if (renderJob == null)
                    return NotFound();

                _dbContext.RenderJobs.Remove(renderJob);
                await _dbContext.SaveChangesAsync();

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
}

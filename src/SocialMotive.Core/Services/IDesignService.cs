using SocialMotive.Core.Data.Generator;

namespace SocialMotive.Core.Services;

/// <summary>
/// Rendering pipeline for the Designer feature area.
/// CRUD operations are handled directly by DesignController; this service owns image generation.
/// </summary>
public interface IDesignService
{
    /// <summary>
    /// Synchronously render a template to PNG bytes.
    /// Text layer variables (e.g. <c>{{event.title}}</c>) are resolved from <paramref name="variables"/>.
    /// </summary>
    Task<byte[]> RenderAsync(int templateId, Dictionary<string, string>? variables = null, CancellationToken ct = default);

    /// <summary>
    /// Queue an async render job. Returns the <see cref="DbRenderJob.RenderJobId"/> immediately;
    /// the background worker will complete the render and persist the result PNG.
    /// </summary>
    Task<int> EnqueueRenderAsync(int templateId, int userId, Dictionary<string, string>? variables = null, CancellationToken ct = default);

    /// <summary>Get a render job by ID, including result PNG bytes if completed.</summary>
    Task<DbRenderJob?> GetRenderJobAsync(int renderJobId, CancellationToken ct = default);
}

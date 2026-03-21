using SocialMotive.Core.Data;
using SocialMotive.Core.Services;

namespace SocialMotive.Designer.Services;

/// <summary>
/// Background worker that drains the <see cref="IBackgroundRenderQueue"/>,
/// invokes <see cref="IDesignService.RenderAsync"/> for each job, and persists
/// the result PNG back to <see cref="DbRenderJob"/>.
/// </summary>
public class RenderWorkerService(
    IBackgroundRenderQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<RenderWorkerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RenderWorkerService started");

        await foreach (var jobId in queue.DequeueAllAsync(stoppingToken))
        {
            await ProcessJobAsync(jobId, stoppingToken);
        }

        logger.LogInformation("RenderWorkerService stopped");
    }

    private async Task ProcessJobAsync(int jobId, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();
        var designService = scope.ServiceProvider.GetRequiredService<IDesignService>();

        var job = await db.RenderJobs.FindAsync([jobId], ct);
        if (job == null)
        {
            logger.LogWarning("Render job {RenderJobId} not found — skipped", jobId);
            return;
        }

        job.Status = "Running";
        job.StartedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        try
        {
            var png = await designService.RenderAsync(job.TemplateId, variables: null, ct);

            job.ImagePng = png;
            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Render job {RenderJobId} completed ({Bytes} bytes)", jobId, png.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Render job {RenderJobId} failed", jobId);

            job.Status = "Failed";
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;

            try { await db.SaveChangesAsync(ct); }
            catch (Exception saveEx) { logger.LogError(saveEx, "Failed to persist error state for job {RenderJobId}", jobId); }
        }
    }
}

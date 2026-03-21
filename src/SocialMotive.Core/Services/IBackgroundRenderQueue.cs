namespace SocialMotive.Core.Services;

/// <summary>
/// Queue for dispatching render job IDs to the background render worker.
/// </summary>
public interface IBackgroundRenderQueue
{
    /// <summary>Enqueue a render job ID for processing.</summary>
    void Enqueue(int renderJobId);

    /// <summary>Asynchronously consume all job IDs as they arrive.</summary>
    IAsyncEnumerable<int> DequeueAllAsync(CancellationToken ct);
}

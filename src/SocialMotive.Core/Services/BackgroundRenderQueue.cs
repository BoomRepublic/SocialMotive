using System.Threading.Channels;

namespace SocialMotive.Core.Services;

/// <summary>
/// Bounded channel-based implementation of <see cref="IBackgroundRenderQueue"/>.
/// Registered as singleton so the single channel instance is shared across all consumers.
/// </summary>
public class BackgroundRenderQueue : IBackgroundRenderQueue
{
    private readonly Channel<int> _channel = Channel.CreateBounded<int>(
        new BoundedChannelOptions(capacity: 100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    public void Enqueue(int renderJobId)
        => _channel.Writer.TryWrite(renderJobId);

    public IAsyncEnumerable<int> DequeueAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}

using SocialMotive.Core.Services;

namespace SocialMotive.LiveMap.Services;

/// <summary>
/// Background service that marks trackers as inactive
/// if no location update has been received for 5 minutes.
/// Runs every 1 minute.
/// </summary>
public class TrackerTimeoutService(IServiceScopeFactory scopeFactory, ILogger<TrackerTimeoutService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var trackerService = scope.ServiceProvider.GetRequiredService<ITrackerService>();
                await trackerService.DeactivateStaleTrackersAsync(Timeout, stoppingToken);
                logger.LogDebug("Stale tracker deactivation cycle completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deactivating stale trackers");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }
}

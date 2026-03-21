using Microsoft.AspNetCore.SignalR;
using SocialMotive.Core.Model.LiveMap;
using SocialMotive.Core.Services;

namespace SocialMotive.LiveMap.Hubs;

/// <summary>
/// SignalR hub for broadcasting tracker locations to connected map clients.
/// On connect, sends the current snapshot so the client sees all markers immediately.
/// </summary>
public class LocationHub(LocationCacheService cache, IServiceScopeFactory scopeFactory) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var snapshot = cache.GetAll();

        // If cache is empty (first client after app start), load from DB
        if (snapshot.Count == 0)
        {
            using var scope = scopeFactory.CreateScope();
            var trackerService = scope.ServiceProvider.GetRequiredService<ITrackerService>();
            snapshot = await trackerService.GetLatestLocationsForActiveTrackersAsync();
            cache.LoadSnapshot(snapshot);
        }

        await Clients.Caller.SendAsync("ReceiveLocations", snapshot);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Returns the current location snapshot. Clients can invoke this after
    /// the connection is fully established to reliably get the initial markers.
    /// Loads from DB if the cache is empty (race with OnConnectedAsync).
    /// </summary>
    public async Task<List<TrackerLocation>> GetLocations()
    {
        var locations = cache.GetAll();
        if (locations.Count > 0)
            return locations;

        using var scope = scopeFactory.CreateScope();
        var trackerService = scope.ServiceProvider.GetRequiredService<ITrackerService>();
        locations = await trackerService.GetLatestLocationsForActiveTrackersAsync();
        cache.LoadSnapshot(locations);
        return locations;
    }
}

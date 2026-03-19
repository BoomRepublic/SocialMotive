using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.LiveMap;
using SocialMotive.LiveMap.Services;

namespace SocialMotive.LiveMap.Hubs;

/// <summary>
/// SignalR hub for broadcasting tracker locations to connected map clients.
/// On connect, sends the current snapshot so the client sees all markers immediately.
/// </summary>
public class LocationHub : Hub
{
    private readonly LocationCacheService _cache;
    private readonly IServiceScopeFactory _scopeFactory;

    public LocationHub(LocationCacheService cache, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    public override async Task OnConnectedAsync()
    {
        var snapshot = _cache.GetAll();

        // If cache is empty (first client after app start), load from DB
        if (snapshot.Count == 0)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

            // Query latest location per tracker using a join to avoid subquery cast issues
            snapshot = await (
                from t in db.Trackers
                join l in db.Locations on t.TrackerId equals l.TrackerId
                where l.LocationId == db.Locations
                    .Where(loc => loc.TrackerId == t.TrackerId)
                    .OrderByDescending(loc => loc.Timestamp)
                    .Select(loc => loc.LocationId)
                    .FirstOrDefault()
                select new TrackerLocation
                {
                    TrackerId = t.TrackerId,
                    DisplayName = t.DisplayName,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude,
                    AccuracyMeters = l.AccuracyMeters,
                    SpeedKmh = l.SpeedKmh,
                    HeadingDegrees = l.HeadingDegrees,
                    Timestamp = l.Timestamp,
                })
                .AsNoTracking()
                .ToListAsync();

            _cache.LoadSnapshot(snapshot);
        }

        await Clients.Caller.SendAsync("ReceiveLocations", snapshot);
        await base.OnConnectedAsync();
    }
}

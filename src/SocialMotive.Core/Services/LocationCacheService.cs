using System.Collections.Concurrent;
using SocialMotive.Core.Model.LiveMap;

namespace SocialMotive.Core.Services;

/// <summary>
/// Thread-safe in-memory cache of the latest location per tracker.
/// Updated by LocationCallbackController when TelegramBot pushes new locations.
/// Read by LocationHub when new clients connect.
/// </summary>
public class LocationCacheService
{
    private readonly ConcurrentDictionary<int, TrackerLocation> _locations = new();

    public List<TrackerLocation> GetAll() => _locations.Values.ToList();

    public void Update(TrackerLocation location)
    {
        _locations.AddOrUpdate(location.TrackerId, location, (_, _) => location);
    }

    public void LoadSnapshot(List<TrackerLocation> locations)
    {
        foreach (var loc in locations)
            _locations.AddOrUpdate(loc.TrackerId, loc, (_, _) => loc);
    }
}

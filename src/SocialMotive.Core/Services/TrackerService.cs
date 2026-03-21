using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.LiveMap;

namespace SocialMotive.Core.Services;

public class TrackerService : ITrackerService
{
    private readonly SocialMotiveDbContext _db;

    public TrackerService(SocialMotiveDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task RecordLocationAsync(int trackerId, double latitude, double longitude,
        double? accuracyMeters, double? headingDegrees, DateTime timestamp, CancellationToken ct = default)
    {
        // FindAsync returns the tracked entity from EF identity map if already loaded — no extra query
        var tracker = await _db.Trackers.FindAsync([trackerId], ct);
        if (tracker == null) return;

        var now = DateTime.UtcNow;
        tracker.IsActive = true;
        tracker.LastUpdateReceivedAt = now;

        _db.Locations.Add(new DbLocation
        {
            TrackerId = trackerId,
            Latitude = latitude,
            Longitude = longitude,
            AccuracyMeters = accuracyMeters,
            HeadingDegrees = headingDegrees,
            SpeedKmh = null,
            Timestamp = timestamp,
            CreatedAt = now,
            ModifiedAt = now
        });

        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeactivateStaleTrackersAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow - timeout;

        var staleTrackers = await _db.Trackers
            .Where(t => t.IsActive && t.LastUpdateReceivedAt != null && t.LastUpdateReceivedAt < cutoff)
            .ToListAsync(ct);

        if (staleTrackers.Count == 0) return;

        foreach (var tracker in staleTrackers)
            tracker.IsActive = false;

        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<DbTracker> CreateTrackerForUserAsync(int userId, string displayName,
        string? email, string? phone, int? cityId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var tracker = new DbTracker
        {
            UserId = userId,
            DisplayName = displayName,
            Email = email,
            Phone = phone,
            CityId = cityId,
            JoinedAt = now,
            CreatedAt = now,
            ModifiedAt = now
        };
        _db.Trackers.Add(tracker);
        await _db.SaveChangesAsync(ct);
        return tracker;
    }

    /// <inheritdoc />
    public async Task<List<TrackerLocation>> GetLatestLocationsForActiveTrackersAsync(CancellationToken ct = default)
    {
        return await (
            from t in _db.Trackers
            join l in _db.Locations on t.TrackerId equals l.TrackerId
            where t.IsActive
            && l.LocationId == _db.Locations
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
            .ToListAsync(ct);
    }
}

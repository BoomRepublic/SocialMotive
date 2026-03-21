using SocialMotive.Core.Data;
using SocialMotive.Core.Model.LiveMap;

namespace SocialMotive.Core.Services;

/// <summary>
/// Business logic for tracker lifecycle: location recording, activation, deactivation, and creation.
/// </summary>
public interface ITrackerService
{
    /// <summary>
    /// Record a new GPS location for a tracker, mark it active, and update LastUpdateReceivedAt.
    /// </summary>
    Task RecordLocationAsync(int trackerId, double latitude, double longitude,
        double? accuracyMeters, double? headingDegrees, DateTime timestamp, CancellationToken ct = default);

    /// <summary>
    /// Set IsActive = false for all trackers with no update within the given timeout window.
    /// </summary>
    Task DeactivateStaleTrackersAsync(TimeSpan timeout, CancellationToken ct = default);

    /// <summary>
    /// Create a new tracker linked to an existing user.
    /// </summary>
    Task<DbTracker> CreateTrackerForUserAsync(int userId, string displayName,
        string? email, string? phone, int? cityId, CancellationToken ct = default);

    /// <summary>
    /// Get the latest location per active tracker, used for map snapshots on client connect.
    /// </summary>
    Task<List<TrackerLocation>> GetLatestLocationsForActiveTrackersAsync(CancellationToken ct = default);
}

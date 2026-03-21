# Tracker Architecture

## Projects Involved

| Project | Role |
|---|---|
| `SocialMotive.TelegramBot` | Receives Telegram live location messages; calls `ITrackerService` to record them; notifies LiveMap |
| `SocialMotive.Core` | Entities (`DbTracker`, `DbLocation`), DTO (`TrackerLocation`), `ITrackerService` + `TrackerService`, `LocationCacheService` |
| `SocialMotive.LiveMap` | SignalR hub, in-memory cache, stale-tracker background service, Leaflet map |

## End-to-End Flow

```
Telegram user shares live location
  └─ TelegramUpdateHandler.HandleLocationUpdateAsync()
       ├─ Looks up UserSocialAccount → Tracker by chatId/UserId
       ├─ ITrackerService.RecordLocationAsync()
       │    ├─ Inserts DbLocation
       │    └─ Sets DbTracker.IsActive = true, LastUpdateReceivedAt = now
       └─ Fire-and-forget POST to LiveMap /api/location-update
            └─ LocationCallbackController
                 ├─ LocationCacheService.Update()       ← in-memory cache
                 └─ SignalR hub broadcast ReceiveLocations
                      └─ leafletMap.js updates Leaflet markers
```

On client (re)connect:
```
Browser connects to /locationhub
  └─ LocationHub.OnConnectedAsync()
       ├─ LocationCacheService.GetAll()  ← fast path (cache warm)
       └─ ITrackerService.GetLatestLocationsForActiveTrackersAsync()  ← DB fallback (cache empty)
            └─ Sends ReceiveLocations to caller
```

## Key Files

| File | Purpose |
|---|---|
| `Services/ITrackerService.cs` | Contract for all tracker business logic |
| `Services/TrackerService.cs` | Implementation — DB operations only, no framework deps |
| `Services/LocationCacheService.cs` | Thread-safe in-memory latest-location cache (Singleton) |
| `Data/DbTracker.cs` | Tracker entity |
| `Data/DbLocation.cs` | GPS location record entity |
| `Model/LiveMap/TrackerLocation.cs` | Lightweight DTO for SignalR broadcasts and map snapshots |

## Design Decisions

### IsActive vs IsLive
- `IsActive` — managed by code: set `true` when a location arrives, set `false` by `TrackerTimeoutService` after 5 minutes of silence. Used in all queries to filter the map.
- `IsLive` — stored in DB but currently unused. Reserved for a future "manual broadcast" mode distinct from GPS tracking.

### 5-Minute Stale Timeout
`TrackerTimeoutService` (LiveMap) runs every 1 minute and calls `ITrackerService.DeactivateStaleTrackersAsync(TimeSpan.FromMinutes(5))`. The 5-minute value is hardcoded in the service; it is passed as a parameter to `DeactivateStaleTrackersAsync` so it can be varied per caller if needed.

### SSR + Browser SignalR Pattern
`LeafletMap.razor` renders initial state server-side (calls `ITrackerService` directly during `OnInitializedAsync`). All live updates are handled by `leafletMap.js` in the browser, which connects to `/locationhub`. **Never create a server-side `HubConnection` from a Blazor page** — updates flow from SignalR server to browser JS, not to Blazor server components.

### Fire-and-Forget HTTP Notification
After recording a location to the DB, `TelegramUpdateHandler` sends a `POST /api/location-update` to LiveMap using the `"LiveMapApi"` named `HttpClient`. This is fire-and-forget (`Task.Run`): failures are logged as warnings but do not block or fail the location save. Map lag (a missed broadcast) is acceptable; the DB is always the source of truth.

### Cache vs DB
`LocationCacheService` holds the latest position per tracker in memory for fast SignalR broadcasts. On app restart or first client connect, `LocationHub` falls back to `ITrackerService.GetLatestLocationsForActiveTrackersAsync()` to warm the cache from DB. The DB is always authoritative.

### Tracker Auto-Creation
When a Telegram user completes `/register` or `/link`, a `DbTracker` is automatically created via `ITrackerService.CreateTrackerForUserAsync`. No admin action is needed to start tracking — but admin assigns group/role afterward.

## ITrackerService Contract

```csharp
// Record a GPS location and activate the tracker
Task RecordLocationAsync(int trackerId, double latitude, double longitude,
    double? accuracyMeters, double? headingDegrees, DateTime timestamp, CancellationToken ct);

// Deactivate trackers silent for longer than `timeout`
Task DeactivateStaleTrackersAsync(TimeSpan timeout, CancellationToken ct);

// Create a tracker linked to an existing user (called on Telegram registration/link)
Task<DbTracker> CreateTrackerForUserAsync(int userId, string displayName,
    string? email, string? phone, int? cityId, CancellationToken ct);

// Latest location per active tracker — for map snapshots
Task<List<TrackerLocation>> GetLatestLocationsForActiveTrackersAsync(CancellationToken ct);
```

`TrackerService` is **Scoped**. `LocationCacheService` is **Singleton**.

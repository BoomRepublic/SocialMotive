using System.Diagnostics;
using System.Net.Http.Json;
using SocialMotive.Core.Model.Public;

namespace SocialMotive.Core.Services;

/// <summary>
/// Service for making Public API calls (no auth required)
/// </summary>
public class PublicApiService
{
    private readonly HttpClient _http;

    public PublicApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("PublicApi");
    }

    #region Events

    /// <summary>
    /// Get list of upcoming published events with optional filters
    /// </summary>
    public async Task<List<Event>> GetEventsAsync(string? city = null, int? eventTypeId = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(city))
                queryParams.Add($"city={Uri.EscapeDataString(city)}");
            if (eventTypeId.HasValue)
                queryParams.Add($"eventTypeId={eventTypeId.Value}");

            var url = "public/events";
            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await _http.GetFromJsonAsync<List<Event>>(url) ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting public events: {ex.Message}");
            return new();
        }
    }

    /// <summary>
    /// Get single published event by ID with full details
    /// </summary>
    public async Task<EventDetail?> GetEventAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<EventDetail>($"public/events/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting public event {id}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Event Types

    /// <summary>
    /// Get list of event types for filtering
    /// </summary>
    public async Task<List<EventType>> GetEventTypesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<EventType>>("public/event-types") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting public event types: {ex.Message}");
            return new();
        }
    }

    #endregion

    #region Users

    /// <summary>
    /// Get public profile for a user by username
    /// </summary>
    public async Task<UserProfile?> GetUserProfileAsync(string username)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserProfile>($"public/profile/{Uri.EscapeDataString(username)}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting public profile for {username}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Cities

    /// <summary>
    /// Get list of cities for location filtering
    /// </summary>
    public async Task<List<City>> GetCitiesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<City>>("public/cities") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting public cities: {ex.Message}");
            return new();
        }
    }

    #endregion
}

namespace SocialMotive.Frontend.Web.Services;

/// <summary>
/// Typed API service for dashboard operations
/// </summary>
public interface IDashboardApiService
{
    Task<dynamic?> GetUserProfileAsync(CancellationToken cancellationToken = default);
    Task<dynamic?> GetEventsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<dynamic?> GetEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task<dynamic?> RegisterEventAsync(int eventId, int[] taskIds, CancellationToken cancellationToken = default);
}

public class DashboardApiService : IDashboardApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardApiService> _logger;

    public DashboardApiService(HttpClient httpClient, ILogger<DashboardApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<dynamic?> GetUserProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/dashboard/user-profile", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile");
            throw;
        }
    }

    public async Task<dynamic?> GetEventsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/dashboard/events?page={page}&pageSize={pageSize}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events");
            throw;
        }
    }

    public async Task<dynamic?> GetEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/dashboard/events/{eventId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event {EventId}", eventId);
            throw;
        }
    }

    public async Task<dynamic?> RegisterEventAsync(int eventId, int[] taskIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { taskIds }),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync($"/api/dashboard/events/{eventId}/register", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering for event {EventId}", eventId);
            throw;
        }
    }
}

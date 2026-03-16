using System.Diagnostics;
using System.Net.Http.Json;
using SocialMotive.Core.Model.Volunteer;

namespace SocialMotive.Core.Services;

/// <summary>
/// Service for making Volunteer API calls from Blazor components.
/// All endpoints require authentication; auth cookies are forwarded automatically.
/// </summary>
public class VolunteerApiService
{
    private readonly HttpClient _http;

    public VolunteerApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("VolunteerApi");
    }

    #region Profile

    /// <summary>
    /// Get the authenticated volunteer's own profile
    /// </summary>
    public async Task<Profile?> GetProfileAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<Profile>("profile");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting volunteer profile: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Update the authenticated volunteer's own profile
    /// </summary>
    public async Task<Profile?> UpdateProfileAsync(ProfileUpdate update)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("profile", update);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Profile>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating volunteer profile: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Event Participation

    /// <summary>
    /// Get the volunteer's event participations
    /// </summary>
    public async Task<List<Participation>> GetParticipationsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Participation>>("participations") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting participations: {ex.Message}");
            return new();
        }
    }

    /// <summary>
    /// Join an event
    /// </summary>
    public async Task<Participation?> JoinEventAsync(int eventId)
    {
        try
        {
            var response = await _http.PostAsync($"participations/{eventId}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Participation>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error joining event {eventId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Leave an event
    /// </summary>
    public async Task<bool> LeaveEventAsync(int eventId)
    {
        try
        {
            var response = await _http.DeleteAsync($"participations/{eventId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error leaving event {eventId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Update review and rating for an event
    /// </summary>
    public async Task<Participation?> UpdateReviewAsync(int eventId, ParticipantReviewUpdate update)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"participations/{eventId}/review", update);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Participation>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating review for event {eventId}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Task Assignments

    /// <summary>
    /// Get the volunteer's task assignments
    /// </summary>
    public async Task<List<TaskAssignment>> GetTaskAssignmentsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<TaskAssignment>>("task-assignments") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting task assignments: {ex.Message}");
            return new();
        }
    }

    /// <summary>
    /// Sign up for an event task
    /// </summary>
    public async Task<TaskAssignment?> SignUpForTaskAsync(int eventTaskId)
    {
        try
        {
            var response = await _http.PostAsync($"task-assignments/{eventTaskId}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskAssignment>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error signing up for task {eventTaskId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Update notes on a task assignment
    /// </summary>
    public async Task<TaskAssignment?> UpdateTaskAssignmentAsync(int eventTaskId, TaskAssignmentUpdate update)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"task-assignments/{eventTaskId}", update);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskAssignment>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating task assignment {eventTaskId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Withdraw from a task assignment
    /// </summary>
    public async Task<bool> WithdrawFromTaskAsync(int eventTaskId)
    {
        try
        {
            var response = await _http.DeleteAsync($"task-assignments/{eventTaskId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error withdrawing from task {eventTaskId}: {ex.Message}");
            return false;
        }
    }

    #endregion
}

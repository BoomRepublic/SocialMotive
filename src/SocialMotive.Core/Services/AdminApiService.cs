using System.Diagnostics;
using System.Net.Http.Json;
using SocialMotive.Core.Model.Admin;

namespace SocialMotive.Core.Services;

/// <summary>
/// Service for making Admin API calls to the backend
/// </summary>
public class AdminApiService
{
    private readonly HttpClient _http;

    public AdminApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("AdminApi");
    }

    #region Users

    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<User>>("users") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting users: {ex.Message}");
            return new();
        }
    }

    public async Task<User?> GetUserAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<User>($"user/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<User?> CreateUserAsync(User user)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("user", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating user: {ex.Message}");
            return null;
        }
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"user/{id}", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating user {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"user/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting user {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Trackers

    public async Task<List<Tracker>> GetTrackersAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Tracker>>("trackers") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting trackers: {ex.Message}");
            return new();
        }
    }

    public async Task<Tracker?> GetTrackerAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Tracker>($"tracker/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting tracker {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Tracker?> CreateTrackerAsync(Tracker tracker)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("tracker", tracker);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Tracker>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating tracker: {ex.Message}");
            return null;
        }
    }

    public async Task<Tracker?> UpdateTrackerAsync(int id, TrackerUpdateRequest tracker)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"tracker/{id}", tracker);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Tracker>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating tracker {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteTrackerAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"tracker/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting tracker {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Events

    public async Task<List<Event>> GetEventsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Event>>("events") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting events: {ex.Message}");
            return new();
        }
    }

    public async Task<Event?> GetEventAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Event>($"event/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Event?> CreateEventAsync(Event Event)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("event", Event);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Event>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating event: {ex.Message}");
            return null;
        }
    }

    public async Task<Event?> UpdateEventAsync(int id, Event Event)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"event/{id}", Event);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Event>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating event {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"event/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting event {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Labels

    public async Task<List<Label>> GetLabelsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Label>>("labels") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting labels: {ex.Message}");
            return new();
        }
    }

    public async Task<Label?> GetLabelAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Label>($"label/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting label {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Label?> CreateLabelAsync(Label label)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("label", label);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Label>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating label: {ex.Message}");
            return null;
        }
    }

    public async Task<Label?> UpdateLabelAsync(int id, Label label)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"label/{id}", label);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Label>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating label {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteLabelAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"label/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting label {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region EventTypes

    public async Task<List<EventType>> GetEventTypesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<EventType>>("event-types") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event types: {ex.Message}");
            return new();
        }
    }

    public async Task<EventType?> GetEventTypeAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<EventType>($"event-type/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event type {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<EventType?> CreateEventTypeAsync(EventType eventType)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("event-type", eventType);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventType>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating event type: {ex.Message}");
            return null;
        }
    }

    public async Task<EventType?> UpdateEventTypeAsync(int id, EventType eventType)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"event-type/{id}", eventType);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventType>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating event type {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteEventTypeAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"event-type/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting event type {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region EventSkills

    public async Task<List<EventSkill>> GetEventSkillsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<EventSkill>>("event-skills") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event skills: {ex.Message}");
            return new();
        }
    }

    public async Task<EventSkill?> GetEventSkillAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<EventSkill>($"event-skill/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event skill {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<EventSkill?> CreateEventSkillAsync(EventSkill eventSkill)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("event-skill", eventSkill);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventSkill>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating event skill: {ex.Message}");
            return null;
        }
    }

    public async Task<EventSkill?> UpdateEventSkillAsync(int id, EventSkill eventSkill)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"event-skill/{id}", eventSkill);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventSkill>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating event skill {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteEventSkillAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"event-skill/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting event skill {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region UserSocialAccounts

    public async Task<List<UserSocialAccount>> GetUserSocialAccountsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserSocialAccount>>("user-social-accounts") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user social accounts: {ex.Message}");
            return new();
        }
    }

    public async Task<List<UserSocialAccount>> GetUserSocialAccountsByUserAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserSocialAccount>>($"user/{userId}/social-accounts") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting social accounts for user {userId}: {ex.Message}");
            return new();
        }
    }

    public async Task<UserSocialAccount?> GetUserSocialAccountAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserSocialAccount>($"user-social-account/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user social account {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<UserSocialAccount?> CreateUserSocialAccountAsync(UserSocialAccount account)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("user-social-account", account);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserSocialAccount>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating user social account: {ex.Message}");
            return null;
        }
    }

    public async Task<UserSocialAccount?> UpdateUserSocialAccountAsync(int id, UserSocialAccount account)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"user-social-account/{id}", account);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserSocialAccount>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating user social account {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteUserSocialAccountAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"user-social-account/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting user social account {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region SocialPlatforms

    public async Task<List<SocialPlatform>> GetSocialPlatformsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<SocialPlatform>>("social-platforms") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting social platforms: {ex.Message}");
            return new();
        }
    }

    public async Task<SocialPlatform?> GetSocialPlatformAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<SocialPlatform>($"social-platform/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting social platform {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<SocialPlatform?> CreateSocialPlatformAsync(SocialPlatform platform)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("social-platform", platform);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SocialPlatform>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating social platform: {ex.Message}");
            return null;
        }
    }

    public async Task<SocialPlatform?> UpdateSocialPlatformAsync(int id, SocialPlatform platform)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"social-platform/{id}", platform);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SocialPlatform>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating social platform {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteSocialPlatformAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"social-platform/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting social platform {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Groups

    public async Task<List<Group>> GetGroupsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Group>>("groups") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting groups: {ex.Message}");
            return new();
        }
    }

    public async Task<Group?> GetGroupAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Group>($"group/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting group {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Group?> CreateGroupAsync(Group group)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("group", group);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Group>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating group: {ex.Message}");
            return null;
        }
    }

    public async Task<Group?> UpdateGroupAsync(int id, Group group)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"group/{id}", group);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Group>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating group {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteGroupAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"group/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting group {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Cities

    public async Task<List<City>> GetCitiesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<City>>("cities") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting cities: {ex.Message}");
            return new();
        }
    }

    public async Task<City?> GetCityAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<City>($"city/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting city {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<City?> CreateCityAsync(City city)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("city", city);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<City>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating city: {ex.Message}");
            return null;
        }
    }

    public async Task<City?> UpdateCityAsync(int id, City city)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"city/{id}", city);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<City>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating city {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"city/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting city {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Locations

    public async Task<List<Location>> GetLocationsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Location>>("locations") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting locations: {ex.Message}");
            return new();
        }
    }

    public async Task<List<Location>> GetLocationsByTrackerAsync(int trackerId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Location>>($"tracker/{trackerId}/locations") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting locations for tracker {trackerId}: {ex.Message}");
            return new();
        }
    }

    public async Task<Location?> GetLocationAsync(long id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Location>($"location/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting location {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Location?> CreateLocationAsync(Location location)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("location", location);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Location>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating location: {ex.Message}");
            return null;
        }
    }

    public async Task<Location?> UpdateLocationAsync(long id, Location location)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"location/{id}", location);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Location>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating location {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteLocationAsync(long id)
    {
        try
        {
            var response = await _http.DeleteAsync($"location/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting location {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Settings

    public async Task<List<Setting>> GetSettingsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Setting>>("settings") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting settings: {ex.Message}");
            return new();
        }
    }

    public async Task<Setting?> GetSettingAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Setting>($"setting/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting setting {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Setting?> CreateSettingAsync(Setting setting)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("setting", setting);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Setting>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating setting: {ex.Message}");
            return null;
        }
    }

    public async Task<Setting?> UpdateSettingAsync(int id, Setting setting)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"setting/{id}", setting);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Setting>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating setting {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteSettingAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"setting/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting setting {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region TrackerRoles

    public async Task<List<TrackerRole>> GetTrackerRolesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<TrackerRole>>("tracker-roles") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting tracker roles: {ex.Message}");
            return new();
        }
    }

    public async Task<TrackerRole?> GetTrackerRoleAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<TrackerRole>($"tracker-role/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting tracker role {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<TrackerRole?> CreateTrackerRoleAsync(TrackerRole role)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("tracker-role", role);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TrackerRole>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating tracker role: {ex.Message}");
            return null;
        }
    }

    public async Task<TrackerRole?> UpdateTrackerRoleAsync(int id, TrackerRole role)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"tracker-role/{id}", role);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TrackerRole>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating tracker role {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteTrackerRoleAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"tracker-role/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting tracker role {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region EventTasks

    public async Task<List<EventTask>> GetEventTasksAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<EventTask>>("event-tasks") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event tasks: {ex.Message}");
            return new();
        }
    }

    public async Task<List<EventTask>> GetEventTasksByEventAsync(int eventId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<EventTask>>($"event/{eventId}/tasks") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting tasks for event {eventId}: {ex.Message}");
            return new();
        }
    }

    public async Task<EventTask?> GetEventTaskAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<EventTask>($"event-task/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting event task {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<EventTask?> CreateEventTaskAsync(EventTask task)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("event-task", task);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventTask>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating event task: {ex.Message}");
            return null;
        }
    }

    public async Task<EventTask?> UpdateEventTaskAsync(int id, EventTask task)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"event-task/{id}", task);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EventTask>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating event task {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteEventTaskAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"event-task/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting event task {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Invites

    public async Task<List<Invite>> GetInvitesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Invite>>("invites") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting invites: {ex.Message}");
            return new();
        }
    }

    public async Task<Invite?> GetInviteAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Invite>($"invite/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting invite {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Invite?> CreateInviteAsync(Invite invite)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("invite", invite);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Invite>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating invite: {ex.Message}");
            return null;
        }
    }

    public async Task<Invite?> UpdateInviteAsync(int id, Invite invite)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"invite/{id}", invite);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Invite>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating invite {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteInviteAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"invite/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting invite {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region OrganizationRoles

    public async Task<List<OrganizationRole>> GetOrganizationRolesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<OrganizationRole>>("organization-roles") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organization roles: {ex.Message}");
            return new();
        }
    }

    public async Task<OrganizationRole?> GetOrganizationRoleAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<OrganizationRole>($"organization-role/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organization role {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<OrganizationRole?> CreateOrganizationRoleAsync(OrganizationRole role)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("organization-role", role);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationRole>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating organization role: {ex.Message}");
            return null;
        }
    }

    public async Task<OrganizationRole?> UpdateOrganizationRoleAsync(int id, OrganizationRole role)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"organization-role/{id}", role);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationRole>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating organization role {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteOrganizationRoleAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"organization-role/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting organization role {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Organizations

    public async Task<List<Organization>> GetOrganizationsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Organization>>("organizations") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organizations: {ex.Message}");
            return new();
        }
    }

    public async Task<Organization?> GetOrganizationAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Organization>($"organization/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organization {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Organization?> CreateOrganizationAsync(Organization organization)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("organization", organization);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Organization>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating organization: {ex.Message}");
            return null;
        }
    }

    public async Task<Organization?> UpdateOrganizationAsync(int id, Organization organization)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"organization/{id}", organization);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Organization>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating organization {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteOrganizationAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"organization/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting organization {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region OrganizationUsers

    public async Task<List<OrganizationUser>> GetOrganizationUsersAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<OrganizationUser>>("organization-users") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organization users: {ex.Message}");
            return new();
        }
    }

    public async Task<OrganizationUser?> GetOrganizationUserAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<OrganizationUser>($"organization-user/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting organization user {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<OrganizationUser?> CreateOrganizationUserAsync(OrganizationUser user)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("organization-user", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationUser>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating organization user: {ex.Message}");
            return null;
        }
    }

    public async Task<OrganizationUser?> UpdateOrganizationUserAsync(int id, OrganizationUser user)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"organization-user/{id}", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationUser>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating organization user {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteOrganizationUserAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"organization-user/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting organization user {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Roles

    public async Task<List<Role>> GetRolesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Role>>("roles") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting roles: {ex.Message}");
            return new();
        }
    }

    public async Task<List<UserRole>> GetUserRolesAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserRole>>($"user/{userId}/roles") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user roles for user {userId}: {ex.Message}");
            return new();
        }
    }

    public async Task<List<UserRole>?> UpdateUserRolesAsync(int userId, List<int> roleIds)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"user/{userId}/roles", roleIds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<UserRole>>() ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating roles for user {userId}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region UserGroups

    public async Task<List<UserGroup>> GetUserGroupsAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserGroup>>($"user/{userId}/groups") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user groups for user {userId}: {ex.Message}");
            return new();
        }
    }

    public async Task<List<UserGroup>?> UpdateUserGroupsAsync(int userId, List<int> groupIds)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"user/{userId}/groups", groupIds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<UserGroup>>() ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating groups for user {userId}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region UserLabels

    public async Task<List<UserLabel>> GetUserLabelsAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserLabel>>($"user/{userId}/labels") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting user labels for user {userId}: {ex.Message}");
            return new();
        }
    }

    public async Task<List<UserLabel>?> UpdateUserLabelsAsync(int userId, List<int> labelIds)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"user/{userId}/labels", labelIds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<UserLabel>>() ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating labels for user {userId}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Import

    public async Task<ImportResponse?> ImportDataAsync(string tableName, string jsonData)
    {
        try
        {
            var request = new ImportRequest
            {
                TableName = tableName,
                JsonData = jsonData
            };
            var response = await _http.PostAsJsonAsync("import", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ImportResponse>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error importing data to {tableName}: {ex.Message}");
            return new ImportResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    #endregion
}

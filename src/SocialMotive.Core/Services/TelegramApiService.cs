using System.Diagnostics;
using System.Net.Http.Json;
using SocialMotive.Core.Model.Telegram;

namespace SocialMotive.Core.Services;

/// <summary>
/// Service for making Telegram API calls from Blazor components.
/// Uses the "TelegramApi" named HttpClient with cookie forwarding.
/// </summary>
public class TelegramApiService
{
    private readonly HttpClient _http;

    public TelegramApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("TelegramApi");
    }

    /// <summary>
    /// Generate a link code for linking a Telegram account
    /// </summary>
    public async Task<LinkCode?> GenerateLinkCodeAsync()
    {
        try
        {
            var response = await _http.PostAsync("link", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LinkCode>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error generating Telegram link code: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get the current user's Telegram link status
    /// </summary>
    public async Task<LinkStatus?> GetLinkStatusAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<LinkStatus>("status");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting Telegram link status: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Unlink the current user's Telegram account
    /// </summary>
    public async Task<bool> UnlinkAsync()
    {
        try
        {
            var response = await _http.DeleteAsync("link");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error unlinking Telegram: {ex.Message}");
            return false;
        }
    }
}

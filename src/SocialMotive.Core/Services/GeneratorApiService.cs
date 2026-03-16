using System.Diagnostics;
using System.Net.Http.Json;
using SocialMotive.Core.Model.Generator;

namespace SocialMotive.Core.Services;

/// <summary>
/// Service for making Generator API calls to the backend
/// </summary>
public class GeneratorApiService
{
    private readonly HttpClient _http;

    public GeneratorApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GeneratorApi");
    }

    #region Templates

    public async Task<List<TemplateSummary>> GetTemplatesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<TemplateSummary>>("templates") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting templates: {ex.Message}");
            return new();
        }
    }

    public async Task<TemplateDetail?> GetTemplateAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<TemplateDetail>($"templates/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting template {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<TemplateDetail?> CreateTemplateAsync(CreateTemplateRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("templates", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDetail>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating template: {ex.Message}");
            return null;
        }
    }

    public async Task<TemplateDetail?> UpdateTemplateAsync(int id, UpdateTemplateRequest request)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"templates/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDetail>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating template {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"templates/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting template {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Assets

    public async Task<List<Asset>> GetAssetsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Asset>>("assets") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting assets: {ex.Message}");
            return new();
        }
    }

    public async Task<Asset?> GetAssetAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Asset>($"assets/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting asset {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Asset?> CreateAssetAsync(UploadAssetRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("assets", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Asset>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating asset: {ex.Message}");
            return null;
        }
    }

    public async Task<Asset?> UpdateAssetAsync(int id, Asset Asset)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"assets/{id}", Asset);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Asset>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating asset {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAssetAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"assets/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting asset {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Layers

    public async Task<List<Layer>> GetLayersAsync(int templateId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Layer>>($"templates/{templateId}/layers") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting layers for template {templateId}: {ex.Message}");
            return new();
        }
    }

    public async Task<Layer?> GetLayerAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Layer>($"layers/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting layer {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Layer?> CreateLayerAsync(Layer Layer)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("layers", Layer);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Layer>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating layer: {ex.Message}");
            return null;
        }
    }

    public async Task<Layer?> UpdateLayerAsync(int id, Layer Layer)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"layers/{id}", Layer);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Layer>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating layer {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteLayerAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"layers/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting layer {id}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Render Jobs

    public async Task<List<RenderJobStatus>> GetRenderJobsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<RenderJobStatus>>("renderjobs") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting render jobs: {ex.Message}");
            return new();
        }
    }

    public async Task<RenderJobStatus?> GetRenderJobAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<RenderJobStatus>($"renderjobs/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting render job {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteRenderJobAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"renderjobs/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting render job {id}: {ex.Message}");
            return false;
        }
    }

    #endregion
}

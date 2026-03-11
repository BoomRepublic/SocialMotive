namespace SocialMotive.TrekkerGenerator.Web.Services;

/// <summary>
/// Typed API service for template operations
/// </summary>
public interface ITemplatesApiService
{
    Task<dynamic?> GetTemplatesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<dynamic?> GetTemplateAsync(int id, CancellationToken cancellationToken = default);
    Task<dynamic?> CreateTemplateAsync(dynamic template, CancellationToken cancellationToken = default);
    Task<dynamic?> UpdateTemplateAsync(int id, dynamic template, CancellationToken cancellationToken = default);
    Task DeleteTemplateAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Typed API service for asset operations
/// </summary>
public interface IAssetsApiService
{
    Task<dynamic?> UploadAssetAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<dynamic?> GetAssetAsync(int assetId, CancellationToken cancellationToken = default);
    Task DeleteAssetAsync(int assetId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Typed API service for render/export operations
/// </summary>
public interface IRenderApiService
{
    Task<byte[]?> RenderToPngAsync(dynamic canvasDocument, dynamic exportOptions, CancellationToken cancellationToken = default);
}

// Implementations

public class TemplatesApiService : ITemplatesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TemplatesApiService> _logger;

    public TemplatesApiService(HttpClient httpClient, ILogger<TemplatesApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<dynamic?> GetTemplatesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/templates?page={page}&pageSize={pageSize}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching templates");
            throw;
        }
    }

    public async Task<dynamic?> GetTemplateAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/templates/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching template {TemplateId}", id);
            throw;
        }
    }

    public async Task<dynamic?> CreateTemplateAsync(dynamic template, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(template),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync("/api/templates", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            throw;
        }
    }

    public async Task<dynamic?> UpdateTemplateAsync(int id, dynamic template, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(template),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PutAsync($"/api/templates/{id}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", id);
            throw;
        }
    }

    public async Task DeleteTemplateAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/templates/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", id);
            throw;
        }
    }
}

public class AssetsApiService : IAssetsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AssetsApiService> _logger;

    public AssetsApiService(HttpClient httpClient, ILogger<AssetsApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<dynamic?> UploadAssetAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            
            var response = await _httpClient.PostAsync("/api/assets/upload", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading asset {FileName}", fileName);
            throw;
        }
    }

    public async Task<dynamic?> GetAssetAsync(int assetId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/assets/{assetId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset {AssetId}", assetId);
            throw;
        }
    }

    public async Task DeleteAssetAsync(int assetId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/assets/{assetId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset {AssetId}", assetId);
            throw;
        }
    }
}

public class RenderApiService : IRenderApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RenderApiService> _logger;

    public RenderApiService(HttpClient httpClient, ILogger<RenderApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<byte[]?> RenderToPngAsync(dynamic canvasDocument, dynamic exportOptions, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new { canvasDocument, exportOptions };
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync("/api/render", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering PNG");
            throw;
        }
    }
}

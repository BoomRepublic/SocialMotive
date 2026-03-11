namespace SocialMotive.AdminBackend.Web.Services;

/// <summary>
/// Typed API service for metadata operations
/// </summary>
public interface IMetadataApiService
{
    Task<dynamic?> GetTablesAsync(CancellationToken cancellationToken = default);
    Task<dynamic?> GetTableMetadataAsync(string tableName, CancellationToken cancellationToken = default);
    Task<dynamic?> GetColumnMetadataAsync(string tableName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Typed API service for CRUD operations
/// </summary>
public interface ITableCrudApiService
{
    Task<dynamic?> GetTableDataAsync(string tableName, int page = 1, int pageSize = 25, string? sort = null, string? filter = null, CancellationToken cancellationToken = default);
    Task<dynamic?> CreateRowAsync(string tableName, dynamic row, CancellationToken cancellationToken = default);
    Task<dynamic?> UpdateRowAsync(string tableName, int id, dynamic row, CancellationToken cancellationToken = default);
    Task DeleteRowAsync(string tableName, int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Typed API service for export operations
/// </summary>
public interface IExportApiService
{
    Task<byte[]?> ExportAsync(string tableName, string format, dynamic? filters = null, CancellationToken cancellationToken = default);
}

// Implementations

public class MetadataApiService : IMetadataApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MetadataApiService> _logger;

    public MetadataApiService(HttpClient httpClient, ILogger<MetadataApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<dynamic?> GetTablesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/admin/metadata/tables", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching table list");
            throw;
        }
    }

    public async Task<dynamic?> GetTableMetadataAsync(string tableName, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/admin/metadata/tables/{tableName}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching metadata for table {TableName}", tableName);
            throw;
        }
    }

    public async Task<dynamic?> GetColumnMetadataAsync(string tableName, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/admin/metadata/columns/{tableName}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching column metadata for table {TableName}", tableName);
            throw;
        }
    }
}

public class TableCrudApiService : ITableCrudApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TableCrudApiService> _logger;

    public TableCrudApiService(HttpClient httpClient, ILogger<TableCrudApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<dynamic?> GetTableDataAsync(string tableName, int page = 1, int pageSize = 25, string? sort = null, string? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/api/admin/crud/{tableName}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(sort))
                url += $"&sort={Uri.EscapeDataString(sort)}";
            if (!string.IsNullOrEmpty(filter))
                url += $"&filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data for table {TableName}", tableName);
            throw;
        }
    }

    public async Task<dynamic?> CreateRowAsync(string tableName, dynamic row, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(row),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync($"/api/admin/crud/{tableName}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating row in table {TableName}", tableName);
            throw;
        }
    }

    public async Task<dynamic?> UpdateRowAsync(string tableName, int id, dynamic row, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(row),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PutAsync($"/api/admin/crud/{tableName}/{id}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<dynamic>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating row in table {TableName}", tableName);
            throw;
        }
    }

    public async Task DeleteRowAsync(string tableName, int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/admin/crud/{tableName}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting row from table {TableName}", tableName);
            throw;
        }
    }
}

public class ExportApiService : IExportApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExportApiService> _logger;

    public ExportApiService(HttpClient httpClient, ILogger<ExportApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<byte[]?> ExportAsync(string tableName, string format, dynamic? filters = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new { tableName, format, filters };
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync("/api/admin/export", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data for table {TableName}", tableName);
            throw;
        }
    }
}

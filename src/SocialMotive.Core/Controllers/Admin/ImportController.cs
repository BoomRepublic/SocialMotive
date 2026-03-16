using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Admin;

namespace SocialMotive.Core.Controllers.Admin;

/// <summary>
/// Import Controller for bulk importing JSON data into whitelisted database tables.
/// Requires Admin role.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = AppRoles.Admin)]
public class ImportController : ControllerBase
{
    private readonly ILogger<ImportController> _logger;
    private readonly SocialMotiveDbContext _dbContext;

    public ImportController(
        ILogger<ImportController> logger,
        SocialMotiveDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Generic import endpoint for importing JSON data into any whitelisted table
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ImportResponse>> ImportData([FromBody] ImportRequest request)
    {
        var response = new ImportResponse();
        var results = new List<ImportResult>();

        try
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = "Table name is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.JsonData))
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = "JSON data is required"
                });
            }

            // Map table names to entity types
            var tableTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "Users", typeof(DbUser) },
                { "Trackers", typeof(DbTracker) },
                { "Events", typeof(DbEvent) },
                { "EventTypes", typeof(DbEventType) },
                { "EventTasks", typeof(DbEventTask) },
                { "EventParticipants", typeof(DbEventParticipant) },
                { "EventTaskAssignments", typeof(DbEventTaskAssignment) },
                { "Groups", typeof(DbGroup) },
                { "Labels", typeof(DbLabel) },
                { "TrackerLabels", typeof(DbTrackerLabel) },
                { "TrackerRoles", typeof(DbTrackerRole) },
                { "Cities", typeof(DbCity) },
                { "Locations", typeof(DbLocation) },
                { "Invites", typeof(DbInvite) },
                { "Settings", typeof(DbSetting) },
                { "UserSocialAccounts", typeof(DbUserSocialAccount) }
            };

            if (!tableTypeMap.TryGetValue(request.TableName, out var entityType))
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = $"Table '{request.TableName}' is not supported for import"
                });
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            using var jsonDoc = JsonDocument.Parse(request.JsonData);
            var rootElement = jsonDoc.RootElement;

            if (rootElement.ValueKind != JsonValueKind.Array)
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = "JSON data must be an array"
                });
            }

            var dbSetProperty = _dbContext.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                    p.PropertyType.GetGenericArguments().FirstOrDefault() == entityType);

            if (dbSetProperty == null)
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = $"Could not find DbSet for table '{request.TableName}'"
                });
            }

            var dbSet = dbSetProperty.GetValue(_dbContext);
            var addMethod = dbSet?.GetType().GetMethod("Add");

            if (addMethod == null)
            {
                return BadRequest(new ImportResponse
                {
                    Success = false,
                    Message = "Could not access Add method for the table"
                });
            }

            int rowIndex = 0;
            int successCount = 0;
            int failCount = 0;

            foreach (var element in rootElement.EnumerateArray())
            {
                rowIndex++;
                try
                {
                    var entity = JsonSerializer.Deserialize(element.GetRawText(), entityType, jsonOptions);

                    if (entity == null)
                    {
                        results.Add(new ImportResult { RowIndex = rowIndex, Success = false, Message = "Failed to deserialize row" });
                        failCount++;
                        continue;
                    }

                    addMethod.Invoke(dbSet, new[] { entity });
                    results.Add(new ImportResult { RowIndex = rowIndex, Success = true, Message = "Row added successfully" });
                    successCount++;
                }
                catch (Exception ex)
                {
                    results.Add(new ImportResult
                    {
                        RowIndex = rowIndex,
                        Success = false,
                        Message = $"Error: {ex.InnerException?.Message ?? ex.Message}"
                    });
                    failCount++;
                }
            }

            await _dbContext.SaveChangesAsync();

            response.Success = failCount == 0;
            response.Message = $"Import completed: {successCount} rows imported successfully, {failCount} failed";
            response.Results = results;

            _logger.LogInformation("Import to {TableName} completed: {SuccessCount} success, {FailCount} failed",
                request.TableName, successCount, failCount);

            return Ok(response);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error during import to {TableName}", request.TableName);
            return BadRequest(new ImportResponse
            {
                Success = false,
                Message = $"JSON parsing error: {ex.Message}",
                Results = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during import to {TableName}", request.TableName);
            return BadRequest(new ImportResponse
            {
                Success = false,
                Message = $"Import error: {ex.Message}",
                Results = results
            });
        }
    }
}

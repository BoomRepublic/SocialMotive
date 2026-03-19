using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Telegram;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Telegram Controller for linking Telegram accounts and managing live location tracking.
/// Endpoints are scoped to the authenticated user for link/unlink/status operations.
/// The bot callback endpoint is used internally by the Telegram bot service.
/// </summary>
[ApiController]
[Route("api/telegram")]
[Authorize(Roles = AppRoles.Volunteer + "," + AppRoles.Organizer + "," + AppRoles.Admin)]
public class TelegramController : ControllerBase
{
    /// <summary>
    /// Telegram SocialPlatformId as stored in the SocialPlatforms table
    /// </summary>
    private const int TelegramPlatformId = 8;

    /// <summary>
    /// In-memory store for pending link codes: Code → (UserId, ExpiresAtUtc)
    /// </summary>
    private static readonly ConcurrentDictionary<string, (int UserId, DateTime ExpiresAtUtc)> _pendingLinks = new();

    /// <summary>
    /// Link code validity duration
    /// </summary>
    private static readonly TimeSpan LinkCodeExpiry = TimeSpan.FromMinutes(10);

    private readonly ILogger<TelegramController> _logger;
    private readonly SocialMotiveDbContext _dbContext;

    public TelegramController(
        ILogger<TelegramController> logger,
        SocialMotiveDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Extract the authenticated user's ID from claims
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("UserId")?.Value;

        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found in claims");
    }

    #region Link Management

    /// <summary>
    /// Generate a one-time code the user sends to the Telegram bot to link their account.
    /// Codes expire after 10 minutes.
    /// </summary>
    [HttpPost("link")]
    public async Task<ActionResult<LinkCode>> GenerateLinkCode()
    {
        try
        {
            var userId = GetCurrentUserId();

            // Check if already linked
            var existing = await _dbContext.UserSocialAccounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.SocialPlatformId == TelegramPlatformId);

            if (existing != null)
                return BadRequest(new { message = "Telegram account is already linked. Unlink first to re-link." });

            // Remove any existing pending codes for this user
            foreach (var kvp in _pendingLinks)
            {
                if (kvp.Value.UserId == userId)
                    _pendingLinks.TryRemove(kvp.Key, out _);
            }

            // Generate a 6-character alphanumeric code
            var code = GenerateCode();
            var expiresAt = DateTime.UtcNow.Add(LinkCodeExpiry);

            _pendingLinks[code] = (userId, expiresAt);

            return Ok(new LinkCode
            {
                Code = code,
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Telegram link code");
            return BadRequest(new { message = "Error generating link code", error = ex.Message });
        }
    }

    /// <summary>
    /// Get the current Telegram link status for the authenticated user.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<LinkStatus>> GetLinkStatus()
    {
        try
        {
            var userId = GetCurrentUserId();

            var account = await _dbContext.UserSocialAccounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.SocialPlatformId == TelegramPlatformId);

            if (account == null)
            {
                return Ok(new LinkStatus
                {
                    IsLinked = false
                });
            }

            return Ok(new LinkStatus
            {
                IsLinked = true,
                TelegramUsername = account.UserName,
                ExternalId = account.ExternalId,
                Verified = account.Verified,
                LinkedAt = account.Created
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Telegram link status");
            return BadRequest(new { message = "Error getting link status", error = ex.Message });
        }
    }

    /// <summary>
    /// Unlink the authenticated user's Telegram account.
    /// </summary>
    [HttpDelete("link")]
    public async Task<IActionResult> Unlink()
    {
        try
        {
            var userId = GetCurrentUserId();

            var account = await _dbContext.UserSocialAccounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.SocialPlatformId == TelegramPlatformId);

            if (account == null)
                return NotFound(new { message = "No Telegram account is linked" });

            _dbContext.UserSocialAccounts.Remove(account);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User {UserId} unlinked Telegram account", userId);

            return Ok(new { message = "Telegram account unlinked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking Telegram account");
            return BadRequest(new { message = "Error unlinking Telegram account", error = ex.Message });
        }
    }

    #endregion

    #region Bot Callback (called by SocialMotive.TelegramBot service)

    /// <summary>
    /// Redeem a pending link code. Called by the Telegram bot service when a user sends /link {code}.
    /// Returns the UserId if the code is valid, or 404 if invalid/expired.
    /// </summary>
    [HttpPost("redeem-code")]
    [AllowAnonymous]
    public IActionResult RedeemCode([FromBody] string code)
    {
        try
        {
            var now = DateTime.UtcNow;
            // Clean expired entries opportunistically
            foreach (var kvp in _pendingLinks)
            {
                if (kvp.Value.ExpiresAtUtc < now)
                    _pendingLinks.TryRemove(kvp.Key, out _);
            }

            if (_pendingLinks.TryRemove(code.ToUpperInvariant(), out var entry) && entry.ExpiresAtUtc >= now)
                return Ok(new { userId = entry.UserId });

            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming link code");
            return BadRequest(new { message = "Error redeeming code", error = ex.Message });
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Generate a 6-character uppercase alphanumeric code
    /// </summary>
    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // excludes I, O, 0, 1 to avoid ambiguity
        var random = Random.Shared;
        var code = new char[6];
        for (int i = 0; i < 6; i++)
            code[i] = chars[random.Next(chars.Length)];

        return new string(code);
    }

    #endregion
}

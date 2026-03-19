using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.LiveMap;
using SocialMotive.TelegramBot.Models;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SocialMotive.TelegramBot.Services;

/// <summary>
/// Handles incoming Telegram bot updates (messages, location updates, commands).
/// Uses IServiceScopeFactory to create per-update scopes for DbContext access.
/// </summary>
public class TelegramUpdateHandler
{
    private const int TelegramPlatformId = 8;
    private static readonly TimeSpan RegistrationTimeout = TimeSpan.FromMinutes(30);
    private static readonly ConcurrentDictionary<long, RegistrationState> _registrations = new();

    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    public TelegramUpdateHandler(
        ILogger<TelegramUpdateHandler> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>Process an incoming Telegram update</summary>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        try
        {
            if (update.Type == UpdateType.EditedMessage && update.EditedMessage?.Location != null)
            {
                await HandleLocationUpdateAsync(botClient, update.EditedMessage, ct);
                return;
            }

            if (update.Type != UpdateType.Message || update.Message == null)
                return;

            var message = update.Message;

            if (message.Location != null)
            {
                await HandleLocationUpdateAsync(botClient, message, ct);
                return;
            }

            if (message.Text != null)
                await HandleCommandAsync(botClient, message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Telegram update {UpdateId}", update.Id);
        }
    }

    #region Command Routing

    private async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var text = message.Text?.Trim() ?? string.Empty;
        var chatId = message.Chat.Id;

        // Check if user is mid-registration
        if (_registrations.TryGetValue(chatId, out var regState))
        {
            // Check for timeout
            if (DateTime.UtcNow - regState.StartedAtUtc > RegistrationTimeout)
            {
                _registrations.TryRemove(chatId, out _);
                await botClient.SendMessage(chatId,
                    "⏰ Your registration session has expired. Send /register to start again.",
                    cancellationToken: ct);
                return;
            }

            // Allow /cancel during registration
            if (text.StartsWith("/cancel"))
            {
                _registrations.TryRemove(chatId, out _);
                await botClient.SendMessage(chatId,
                    "Registration cancelled.",
                    cancellationToken: ct);
                return;
            }

            // Allow /skip on optional steps
            if (text.StartsWith("/skip"))
            {
                if (regState.Step == RegistrationStep.AwaitingMobilePhone)
                {
                    regState.Step = RegistrationStep.AwaitingCity;
                    await botClient.SendMessage(chatId,
                        "What city are you in? Type a city name to search, or send /skip.",
                        cancellationToken: ct);
                    return;
                }
                if (regState.Step == RegistrationStep.AwaitingCity)
                {
                    regState.Step = RegistrationStep.AwaitingConfirmation;
                    await SendConfirmationSummaryAsync(botClient, chatId, regState, ct);
                    return;
                }

                await botClient.SendMessage(chatId,
                    "This step cannot be skipped. Please provide the requested information or /cancel.",
                    cancellationToken: ct);
                return;
            }

            // Block other commands during registration
            if (text.StartsWith("/"))
            {
                await botClient.SendMessage(chatId,
                    "Please complete your registration first, or send /cancel to abort.",
                    cancellationToken: ct);
                return;
            }

            // Handle registration input
            await HandleRegistrationInputAsync(botClient, message, regState, ct);
            return;
        }

        // Normal command routing
        if (text.StartsWith("/start"))
            await HandleStartCommandAsync(botClient, message, ct);
        else if (text.StartsWith("/register"))
            await HandleRegisterCommandAsync(botClient, message, ct);
        else if (text.StartsWith("/link"))
            await HandleLinkCommandAsync(botClient, message, ct);
        else if (text.StartsWith("/status"))
            await HandleStatusCommandAsync(botClient, message, ct);
        else if (text.StartsWith("/help"))
        {
            await botClient.SendMessage(chatId,
                "📋 Available commands:\n\n" +
                "/register - Create a new SocialMotive account\n" +
                "/link <code> - Link your Telegram to an existing account\n" +
                "/status - Check your current link status\n" +
                "/help - Show this help message\n\n" +
                "To share your location, use Telegram's 📎 → Location → Share Live Location.",
                cancellationToken: ct);
        }
        else
        {
            await botClient.SendMessage(chatId,
                "Unknown command. Use /help to see available commands.",
                cancellationToken: ct);
        }
    }

    #endregion

    #region /start

    private async Task HandleStartCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;

        // Check if already linked
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var existingAccount = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct);

        if (existingAccount != null)
        {
            await botClient.SendMessage(chatId,
                "Welcome back to SocialMotive Bot! 🌍\n\n" +
                "Your Telegram is already linked.\n" +
                "Use /status to see your account details.\n" +
                "Use /help for more info.\n\n" +
                "Share your live location with me to enable GPS tracking.",
                cancellationToken: ct);
        }
        else
        {
            await botClient.SendMessage(chatId,
                "Welcome to SocialMotive Bot! 🌍\n\n" +
                "Use /register to create a new account.\n" +
                "Use /link <code> to link an existing account.\n" +
                "Use /help for more info.",
                cancellationToken: ct);
        }
    }

    #endregion

    #region Registration Flow

    /// <summary>Handle /register — start the registration conversation</summary>
    private async Task HandleRegisterCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;

        // Clean expired registrations opportunistically
        CleanExpiredRegistrations();

        // Check if already linked
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var existingAccount = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct);

        if (existingAccount != null)
        {
            await botClient.SendMessage(chatId,
                "Your Telegram is already linked to a SocialMotive account. Use /status to see details.",
                cancellationToken: ct);
            return;
        }

        // Check if already mid-registration
        if (_registrations.TryGetValue(chatId, out var existing))
        {
            if (DateTime.UtcNow - existing.StartedAtUtc <= RegistrationTimeout)
            {
                await botClient.SendMessage(chatId,
                    "You already have a registration in progress. Continue where you left off, or send /cancel to start over.",
                    cancellationToken: ct);
                return;
            }
            _registrations.TryRemove(chatId, out _);
        }

        // Start new registration
        var state = new RegistrationState
        {
            ChatId = chatId,
            Step = RegistrationStep.AwaitingFirstName,
            TelegramUsername = message.From?.Username,
            StartedAtUtc = DateTime.UtcNow
        };

        _registrations[chatId] = state;

        await botClient.SendMessage(chatId,
            "Let's create your SocialMotive account! 📝\n\nWhat is your first name?",
            cancellationToken: ct);
    }

    /// <summary>Dispatch registration input to the correct step handler</summary>
    private async Task HandleRegistrationInputAsync(ITelegramBotClient botClient, Message message, RegistrationState state, CancellationToken ct)
    {
        var text = message.Text?.Trim() ?? string.Empty;

        switch (state.Step)
        {
            case RegistrationStep.AwaitingFirstName:
                await ProcessFirstNameAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingLastName:
                await ProcessLastNameAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingEmail:
                await ProcessEmailAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingMobilePhone:
                await ProcessMobilePhoneAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingCity:
                await ProcessCityAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingCitySelection:
                await ProcessCitySelectionAsync(botClient, message.Chat.Id, text, state, ct);
                break;
            case RegistrationStep.AwaitingConfirmation:
                await ProcessConfirmationAsync(botClient, message.Chat.Id, text, state, ct);
                break;
        }
    }

    private async Task ProcessFirstNameAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 100)
        {
            await botClient.SendMessage(chatId,
                "Please enter a valid first name (1–100 characters).",
                cancellationToken: ct);
            return;
        }

        state.FirstName = text;
        state.Step = RegistrationStep.AwaitingLastName;

        await botClient.SendMessage(chatId,
            $"Got it, {text}! What is your last name?",
            cancellationToken: ct);
    }

    private async Task ProcessLastNameAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 100)
        {
            await botClient.SendMessage(chatId,
                "Please enter a valid last name (1–100 characters).",
                cancellationToken: ct);
            return;
        }

        state.LastName = text;
        state.Step = RegistrationStep.AwaitingEmail;

        await botClient.SendMessage(chatId,
            "What is your email address?",
            cancellationToken: ct);
    }

    private async Task ProcessEmailAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        var email = text.ToLowerInvariant().Trim();

        if (email.Length > 255 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await botClient.SendMessage(chatId,
                "That doesn't look like a valid email address. Please try again.",
                cancellationToken: ct);
            return;
        }

        // Check uniqueness
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        if (await dbContext.Users.AnyAsync(u => u.Email == email, ct))
        {
            await botClient.SendMessage(chatId,
                "This email is already registered. If you already have an account, use /cancel and then /link instead.\n\nOr try a different email address.",
                cancellationToken: ct);
            return;
        }

        state.Email = email;
        state.Step = RegistrationStep.AwaitingMobilePhone;

        await botClient.SendMessage(chatId,
            "What is your mobile phone number? (Send /skip to skip this step)",
            cancellationToken: ct);
    }

    private async Task ProcessMobilePhoneAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        var phone = text.Trim();

        if (phone.Length > 20 || !Regex.IsMatch(phone, @"^[\d\s\+\-\(\)]+$"))
        {
            await botClient.SendMessage(chatId,
                "Please enter a valid phone number (digits, +, -, spaces allowed, max 20 characters), or /skip.",
                cancellationToken: ct);
            return;
        }

        state.MobilePhone = phone;
        state.Step = RegistrationStep.AwaitingCity;

        await botClient.SendMessage(chatId,
            "What city are you in? Type a city name to search, or send /skip.",
            cancellationToken: ct);
    }

    private async Task ProcessCityAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        var search = text.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            await botClient.SendMessage(chatId,
                "Please type a city name to search, or send /skip.",
                cancellationToken: ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var matches = await dbContext.Cities
            .Where(c => c.Name != null && c.Name.Contains(search))
            .OrderBy(c => c.Name)
            .Take(5)
            .Select(c => new { c.CityId, c.Name })
            .ToListAsync(ct);

        if (matches.Count == 0)
        {
            await botClient.SendMessage(chatId,
                "No cities found matching your search. Try a different name, or send /skip.",
                cancellationToken: ct);
            return;
        }

        state.CitySearchResults = matches.Select(m => (m.CityId, m.Name ?? "")).ToList();
        state.Step = RegistrationStep.AwaitingCitySelection;

        var list = string.Join("\n", state.CitySearchResults.Select((c, i) => $"{i + 1}. {c.CityName}"));

        await botClient.SendMessage(chatId,
            $"Found these cities:\n\n{list}\n\nSend the number of your city, or /skip.",
            cancellationToken: ct);
    }

    private async Task ProcessCitySelectionAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        if (!int.TryParse(text.Trim(), out var choice) || choice < 1 || choice > state.CitySearchResults.Count)
        {
            var list = string.Join("\n", state.CitySearchResults.Select((c, i) => $"{i + 1}. {c.CityName}"));
            await botClient.SendMessage(chatId,
                $"Please send a number between 1 and {state.CitySearchResults.Count}, or /skip.\n\n{list}",
                cancellationToken: ct);
            return;
        }

        var selected = state.CitySearchResults[choice - 1];
        state.CityId = selected.CityId;
        state.CityName = selected.CityName;
        state.Step = RegistrationStep.AwaitingConfirmation;

        await SendConfirmationSummaryAsync(botClient, chatId, state, ct);
    }

    private async Task SendConfirmationSummaryAsync(ITelegramBotClient botClient, long chatId, RegistrationState state, CancellationToken ct)
    {
        await botClient.SendMessage(chatId,
            "Please confirm your details:\n\n" +
            $"First name: {state.FirstName}\n" +
            $"Last name: {state.LastName}\n" +
            $"Email: {state.Email}\n" +
            $"Phone: {state.MobilePhone ?? "Not provided"}\n" +
            $"City: {state.CityName ?? "Not provided"}\n" +
            $"Telegram: @{state.TelegramUsername ?? "N/A"}\n\n" +
            "Reply \"yes\" to confirm or \"no\" to cancel.",
            cancellationToken: ct);
    }

    private async Task ProcessConfirmationAsync(ITelegramBotClient botClient, long chatId, string text, RegistrationState state, CancellationToken ct)
    {
        var answer = text.ToLowerInvariant().Trim();

        if (answer is "yes" or "y" or "confirm")
        {
            await CreateRegisteredUserAsync(botClient, chatId, state, ct);
            return;
        }

        if (answer is "no" or "n" or "cancel")
        {
            _registrations.TryRemove(chatId, out _);
            await botClient.SendMessage(chatId,
                "Registration cancelled.",
                cancellationToken: ct);
            return;
        }

        await botClient.SendMessage(chatId,
            "Please reply \"yes\" to confirm or \"no\" to cancel.",
            cancellationToken: ct);
    }

    /// <summary>Create the user, social account, and tracker in the database</summary>
    private async Task CreateRegisteredUserAsync(ITelegramBotClient botClient, long chatId, RegistrationState state, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

            // Re-check email uniqueness (race condition guard)
            if (await dbContext.Users.AnyAsync(u => u.Email == state.Email, ct))
            {
                _registrations.TryRemove(chatId, out _);
                await botClient.SendMessage(chatId,
                    "This email was just registered by someone else. Please send /register to try again with a different email.",
                    cancellationToken: ct);
                return;
            }

            // Re-check chatId not already linked
            if (await dbContext.UserSocialAccounts.AnyAsync(
                a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct))
            {
                _registrations.TryRemove(chatId, out _);
                await botClient.SendMessage(chatId,
                    "This Telegram account was just linked to another user. Use /status to check.",
                    cancellationToken: ct);
                return;
            }

            var now = DateTime.UtcNow;

            // Create user
            var user = new DbUser
            {
                FirstName = state.FirstName!,
                LastName = state.LastName!,
                Email = state.Email!,
                Username = state.TelegramUsername,
                MobilePhone = state.MobilePhone,
                CityId = state.CityId,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = now,
                Modified = now
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);

            // Link Telegram account
            dbContext.UserSocialAccounts.Add(new DbUserSocialAccount
            {
                UserId = user.UserId,
                SocialPlatformId = TelegramPlatformId,
                UserName = state.TelegramUsername ?? state.FirstName!,
                ExternalId = chatId.ToString(),
                Verified = true,
                Created = now,
                Modified = now
            });

            // Create tracker
            dbContext.Trackers.Add(new DbTracker
            {
                UserId = user.UserId,
                DisplayName = $"{state.FirstName} {state.LastName}".Trim(),
                Email = state.Email,
                Phone = state.MobilePhone,
                CityId = state.CityId,
                InviteCode = Guid.NewGuid(),
                QrGuid = Guid.NewGuid(),
                JoinedAt = now,
                CreatedAt = now,
                ModifiedAt = now
            });

            await dbContext.SaveChangesAsync(ct);

            _registrations.TryRemove(chatId, out _);

            _logger.LogInformation("New user registered via Telegram: {Email}, UserId {UserId}, ChatId {ChatId}",
                state.Email, user.UserId, chatId);

            await botClient.SendMessage(chatId,
                "✅ Your SocialMotive account has been created and your Telegram is linked!\n\n" +
                "You can now share your live location with me for GPS tracking.\n" +
                "Use Telegram's 📎 → Location → Share Live Location.",
                cancellationToken: ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true)
        {
            _registrations.TryRemove(chatId, out _);
            _logger.LogWarning(ex, "Duplicate key during Telegram registration for chatId {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "This email is already taken. Please send /register to try again with a different email.",
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _registrations.TryRemove(chatId, out _);
            _logger.LogError(ex, "Error creating user via Telegram registration for chatId {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "Something went wrong during registration. Please try again later.",
                cancellationToken: ct);
        }
    }

    private void CleanExpiredRegistrations()
    {
        var expired = _registrations
            .Where(kvp => DateTime.UtcNow - kvp.Value.StartedAtUtc > RegistrationTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var chatId in expired)
            _registrations.TryRemove(chatId, out _);
    }

    #endregion

    #region /link

    /// <summary>Handle /link {code} — call WebApp to redeem the code and link the account</summary>
    private async Task HandleLinkCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;
        var parts = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts == null || parts.Length < 2)
        {
            await botClient.SendMessage(chatId,
                "Usage: /link <code>\n\nGet your link code from the SocialMotive web app.",
                cancellationToken: ct);
            return;
        }

        var code = parts[1].ToUpperInvariant();

        // Redeem the code via WebApp API (codes are held in WebApp's in-memory store)
        int? userId = null;
        try
        {
            var http = _httpClientFactory.CreateClient("WebApi");
            var response = await http.PostAsJsonAsync("api/telegram/redeem-code", code, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RedeemResponse>(cancellationToken: ct);
                userId = result?.UserId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call WebApp redeem-code endpoint");
        }

        if (userId == null)
        {
            await botClient.SendMessage(chatId,
                "❌ Invalid or expired link code. Please generate a new one from the web app.",
                cancellationToken: ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var existingByChatId = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct);

        if (existingByChatId != null)
        {
            await botClient.SendMessage(chatId,
                "⚠️ This Telegram account is already linked to a SocialMotive user. Unlink it first from the web app.",
                cancellationToken: ct);
            return;
        }

        var existingByUser = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.UserId == userId.Value && a.SocialPlatformId == TelegramPlatformId, ct);

        if (existingByUser != null)
        {
            await botClient.SendMessage(chatId,
                "⚠️ Your SocialMotive account already has a Telegram link. Unlink it first from the web app.",
                cancellationToken: ct);
            return;
        }

        var account = new DbUserSocialAccount
        {
            UserId = userId.Value,
            SocialPlatformId = TelegramPlatformId,
            UserName = message.From?.Username ?? message.From?.FirstName ?? "unknown",
            ExternalId = chatId.ToString(),
            Verified = true,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        dbContext.UserSocialAccounts.Add(account);

        var existingTracker = await dbContext.Trackers
            .FirstOrDefaultAsync(t => t.UserId == userId.Value, ct);

        if (existingTracker == null)
        {
            var user = await dbContext.Users.FindAsync([userId.Value], ct);
            if (user != null)
            {
                dbContext.Trackers.Add(new DbTracker
                {
                    UserId = userId.Value,
                    DisplayName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    Phone = user.MobilePhone,
                    InviteCode = Guid.NewGuid(),
                    QrGuid = Guid.NewGuid(),
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                });
            }
        }

        await dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} linked Telegram chatId {ChatId}", userId.Value, chatId);

        await botClient.SendMessage(chatId,
            "✅ Your Telegram account is now linked to SocialMotive!\n\n" +
            "You can now share your live location with me for GPS tracking.\n" +
            "Use Telegram's 📎 → Location → Share Live Location.",
            cancellationToken: ct);
    }

    #endregion

    #region /status

    private async Task HandleStatusCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var account = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct);

        if (account == null)
        {
            await botClient.SendMessage(chatId,
                "❌ This Telegram account is not linked to any SocialMotive user.\n\nUse /register to create an account or /link <code> to link an existing one.",
                cancellationToken: ct);
            return;
        }

        await botClient.SendMessage(chatId,
            $"✅ Linked to SocialMotive\n" +
            $"Username: {account.UserName}\n" +
            $"Linked since: {account.Created:yyyy-MM-dd HH:mm} UTC",
            cancellationToken: ct);
    }

    #endregion

    #region Location Tracking

    private async Task HandleLocationUpdateAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        _logger.LogInformation("Received location update from Telegram chatId {ChatId}", message.Chat.Id);
        var chatId = message.Chat.Id;

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMotiveDbContext>();

        var account = await dbContext.UserSocialAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == chatId.ToString() && a.SocialPlatformId == TelegramPlatformId, ct);

        if (account == null || account.UserId == null)
            return;

        var tracker = await dbContext.Trackers
            .FirstOrDefaultAsync(t => t.UserId == account.UserId, ct);

        if (tracker == null)
        {
            _logger.LogWarning("No tracker found for user {UserId} — cannot store location", account.UserId);
            return;
        }

        var loc = message.Location!;
        var now = DateTime.UtcNow;

        dbContext.Locations.Add(new DbLocation
        {
            TrackerId = tracker.TrackerId,
            Latitude = (float)loc.Latitude,
            Longitude = (float)loc.Longitude,
            AccuracyMeters = (float?)loc.HorizontalAccuracy,
            HeadingDegrees = loc.Heading != null ? (float?)loc.Heading : null,
            SpeedKmh = null,
            Timestamp = now,
            CreatedAt = now,
            ModifiedAt = now
        });

        await dbContext.SaveChangesAsync(ct);

        _logger.LogDebug("Stored location for tracker {TrackerId}: {Lat}, {Lon}",
            tracker.TrackerId, loc.Latitude, loc.Longitude);

        // Notify LiveMap app of the new location (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                var http = _httpClientFactory.CreateClient("LiveMapApi");
                await http.PostAsJsonAsync("api/location-update", new TrackerLocation
                {
                    TrackerId = tracker.TrackerId,
                    DisplayName = tracker.DisplayName,
                    Latitude = (float)loc.Latitude,
                    Longitude = (float)loc.Longitude,
                    AccuracyMeters = (float?)loc.HorizontalAccuracy,
                    SpeedKmh = null,
                    HeadingDegrees = loc.Heading != null ? (float?)loc.Heading : null,
                    Timestamp = now,
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to notify LiveMap of location update for tracker {TrackerId}", tracker.TrackerId);
            }
        });
    }

    #endregion

    private record RedeemResponse(int UserId);
}

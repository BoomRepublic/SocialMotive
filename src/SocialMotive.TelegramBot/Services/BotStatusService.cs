namespace SocialMotive.TelegramBot.Services;

/// <summary>
/// Singleton that tracks live bot status, populated by TelegramBotService on startup.
/// </summary>
public class BotStatusService
{
    public bool IsRunning { get; set; }
    public string? BotUsername { get; set; }
    public long? BotId { get; set; }
    public DateTime? StartedAt { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>Active bot client instance, used by WebhookController to dispatch incoming updates.</summary>
    public Telegram.Bot.ITelegramBotClient? Client { get; set; }
}

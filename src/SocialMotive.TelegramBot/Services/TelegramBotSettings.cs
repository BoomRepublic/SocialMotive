namespace SocialMotive.TelegramBot.Services;

/// <summary>
/// Configuration settings for the Telegram bot, bound from appsettings.json "TelegramBot" section
/// </summary>
public class TelegramBotSettings
{
    public const string SectionName = "TelegramBot";

    /// <summary>Bot API token from @BotFather</summary>
    public string BotToken { get; set; } = string.Empty;

    /// <summary>Optional webhook URL. If set and UseWebhook is true, the bot will use webhooks instead of long-polling.</summary>
    public string? WebhookUrl { get; set; }

    /// <summary>Whether to use webhook mode (true) or long-polling mode (false, default)</summary>
    public bool UseWebhook { get; set; } = false;

    /// <summary>Whether the bot is enabled. Set to false to disable bot startup.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Secret token sent by Telegram in X-Telegram-Bot-Api-Secret-Token header to authenticate webhook calls.</summary>
    public string? WebhookSecretToken { get; set; }
}

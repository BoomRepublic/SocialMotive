using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace SocialMotive.TelegramBot.Services;

/// <summary>
/// Background service that runs the Telegram bot using long-polling.
/// Starts automatically when the application starts if TelegramBot:Enabled is true.
/// </summary>
public class TelegramBotService : BackgroundService
{
    private readonly ILogger<TelegramBotService> _logger;
    private readonly TelegramBotSettings _settings;
    private readonly TelegramUpdateHandler _updateHandler;
    private readonly BotStatusService _status;

    public TelegramBotService(
        ILogger<TelegramBotService> logger,
        IOptions<TelegramBotSettings> settings,
        TelegramUpdateHandler updateHandler,
        BotStatusService status)
    {
        _logger = logger;
        _settings = settings.Value;
        _updateHandler = updateHandler;
        _status = status;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _status.ErrorMessage = "Bot is disabled (TelegramBot:Enabled = false).";
            _logger.LogInformation("Telegram bot is disabled (TelegramBot:Enabled = false). Skipping startup.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.BotToken))
        {
            _status.ErrorMessage = "Bot token is not configured.";
            _logger.LogWarning("Telegram bot token is not configured. Set TelegramBot:BotToken in appsettings.json.");
            return;
        }

        var botClient = new TelegramBotClient(_settings.BotToken);

        try
        {
            var me = await botClient.GetMe(stoppingToken);
            _status.Client = botClient;
            _status.BotUsername = me.Username;
            _status.BotId = me.Id;
            _status.StartedAt = DateTime.UtcNow;
            _status.IsRunning = true;
            _logger.LogInformation("Telegram bot started: @{BotUsername} (ID: {BotId})", me.Username, me.Id);
        }
        catch (Exception ex)
        {
            _status.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Failed to connect to Telegram. Check your BotToken.");
            return;
        }

        if (_settings.UseWebhook && !string.IsNullOrWhiteSpace(_settings.WebhookUrl))
        {
            await botClient.SetWebhook(
                url: _settings.WebhookUrl,
                secretToken: _settings.WebhookSecretToken,
                allowedUpdates: [UpdateType.Message, UpdateType.EditedMessage],
                cancellationToken: stoppingToken);
            _logger.LogInformation("Telegram webhook registered: {WebhookUrl}", _settings.WebhookUrl);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        else
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message, UpdateType.EditedMessage],
                DropPendingUpdates = true
            };

            botClient.StartReceiving(
                updateHandler: _updateHandler.HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Telegram bot is receiving updates via long-polling");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Telegram bot polling error");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _status.IsRunning = false;
        _status.Client = null;
        _logger.LogInformation("Telegram bot stopping...");
        await base.StopAsync(cancellationToken);
    }
}

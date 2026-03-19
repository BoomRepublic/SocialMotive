using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialMotive.TelegramBot.Services;
using Telegram.Bot.Types;

namespace SocialMotive.TelegramBot.Controllers;

/// <summary>
/// Receives incoming Telegram update payloads via webhook.
/// </summary>
[ApiController]
[Route("api/telegram")]
public class WebhookController : ControllerBase
{
    private readonly TelegramUpdateHandler _updateHandler;
    private readonly BotStatusService _status;
    private readonly TelegramBotSettings _settings;

    public WebhookController(
        TelegramUpdateHandler updateHandler,
        BotStatusService status,
        IOptions<TelegramBotSettings> settings)
    {
        _updateHandler = updateHandler;
        _status = status;
        _settings = settings.Value;
    }

    /// <summary>Telegram webhook endpoint — receives Update payloads from Telegram servers.</summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Post([FromBody] Update update, CancellationToken ct)
    {
        // Validate secret token if configured
        if (!string.IsNullOrWhiteSpace(_settings.WebhookSecretToken))
        {
            var header = Request.Headers["X-Telegram-Bot-Api-Secret-Token"].FirstOrDefault();
            if (header != _settings.WebhookSecretToken)
                return Unauthorized();
        }

        if (_status.Client == null)
            return StatusCode(503, "Bot client not initialised");

        await _updateHandler.HandleUpdateAsync(_status.Client, update, ct);
        return Ok();
    }
}

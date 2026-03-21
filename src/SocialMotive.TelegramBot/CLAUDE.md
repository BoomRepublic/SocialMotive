# SocialMotive.TelegramBot — AI Assistant Instructions

## Purpose
Standalone ASP.NET Core Web App (.NET 10) that hosts the Telegram bot for SocialMotive.
Runs independently from SocialMotive.WebApp and connects to the same SQL Server database via SocialMotiveDbContext from SocialMotive.Core.

## Folder Structure
```
SocialMotive.TelegramBot/
  Controllers/
    WebhookController.cs       # POST /api/telegram/webhook — receives Telegram updates in webhook mode
  Models/
    RegistrationState.cs       # RegistrationStep enum + RegistrationState class for multi-step registration flow
  Services/
    BotStatusService.cs        # Singleton tracking bot state (username, uptime, client ref)
    TelegramBotService.cs      # BackgroundService — connects to Telegram, starts polling or registers webhook
    TelegramBotSettings.cs     # Config POCO bound from appsettings "TelegramBot" section
    TelegramUpdateHandler.cs   # Handles all incoming Telegram updates (commands, registration, location)
  Program.cs                   # DI, middleware, status page endpoint (GET /)
  appsettings.json             # Base config (polling mode, empty bot token)
  appsettings.Development.json # Dev overrides (bot token, Debug log level)
  appsettings.Production.json  # Production overrides (webhook mode, KeysPath)
```

## Key Controllers (from SocialMotive.Core via AddApplicationPart)
`TelegramController` (`api/telegram`) is loaded from `SocialMotive.Core` — NOT defined here.
It handles user-facing link/status/unlink API endpoints and the internal `redeem-code` endpoint.

## Modes of Operation
- **Long-polling** (default/dev): `TelegramBot:UseWebhook = false` — bot pulls updates from Telegram
- **Webhook** (production): `TelegramBot:UseWebhook = true` — Telegram pushes updates to `POST /api/telegram/webhook`

## Configuration Keys
| Key | Description |
|-----|-------------|
| `TelegramBot:BotToken` | Bot API token from @BotFather |
| `TelegramBot:Enabled` | Set false to disable bot startup (web app still runs) |
| `TelegramBot:UseWebhook` | true = webhook mode, false = long-polling |
| `TelegramBot:WebhookUrl` | Full public URL of the webhook endpoint |
| `TelegramBot:WebhookSecretToken` | Secret validated in `X-Telegram-Bot-Api-Secret-Token` header |
| `WebApiBaseUrl` | Base URL for the "WebApi" HttpClient (points to self in production) |
| `DataProtection:KeysPath` | Shared directory for DataProtection key ring (shared with WebApp) |

## Bot Commands
| Command | Parameters | Description |
|---------|------------|-------------|
| `/start` | None | Welcome message; prompts `/register` if not linked, shows status if linked |
| `/register` | None | Multi-step registration: first name → last name → email → phone (optional) → city search (optional) → confirmation. Creates DbUser, DbUserSocialAccount (auto-link), DbTracker. No role assigned. |
| `/link` | 6-char code | Link Telegram to an existing SocialMotive account via code from web app |
| `/status` | None | Show current link status |
| `/help` | None | List available commands |
| `/cancel` | None | Cancel in-progress registration |
| `/skip` | None | Skip optional registration steps (phone, city) |

## Registration Flow
1. User sends `/start` or `/register` (unlinked users are prompted to register)
2. Bot collects: first name, last name, email (validated + uniqueness check), phone (`/skip`-able), city (text search against Cities table, `/skip`-able)
3. Bot shows summary, user confirms with "yes"/"no"
4. On confirm: creates `DbUser` (random PasswordHash — no web login), `DbUserSocialAccount` (Telegram linked, `Verified = true`), `DbTracker`
5. No role is assigned — admin assigns roles later
- Registration state is held in a static `ConcurrentDictionary<long, RegistrationState>` keyed by chatId
- 30-minute timeout; expired sessions cleaned opportunistically
- `/cancel` available at any step

## Architecture Notes
- `TelegramController` lives in `SocialMotive.Core` but is served by this project via `AddApplicationPart`
- DataProtection keys are shared with `SocialMotive.WebApp` via a common `KeysPath` + `SetApplicationName("SocialMotive")` so cookies issued by WebApp are valid here
- `WebApiBaseUrl` points to this app itself — `TelegramUpdateHandler` calls `POST api/telegram/redeem-code` via HTTP to consume link codes held in `TelegramController`'s in-memory `ConcurrentDictionary`
- `TelegramPlatformId = 8` is hardcoded in both `TelegramController` (Core) and `TelegramUpdateHandler` — keep in sync if changed
- On startup, `TelegramBotService` calls `SetWebhook()` automatically (webhook mode) — no manual BotFather registration needed
- `InviteCode` and `QrGuid` columns were removed from Trackers (migration 010) — do not reference them

## Commands
```bash
# Run locally
dotnet run --launch-profile https

# Publish to production
dotnet publish -c Release -o "c:\sites\telegram.socialmotive.net" --self-contained false

# Stop/start IIS app pool before/after publish
C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:"telegram.socialmotive.nl"
C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:"telegram.socialmotive.nl"
```

## See Also
- `deploy.md` — Full deployment instructions
- `src/SocialMotive.Core/Controllers/TelegramController.cs` — Link/status/unlink API + redeem-code
- `src/SocialMotive.Core/CLAUDE.md` — Core library conventions

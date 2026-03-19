# Deployment — SocialMotive.TelegramBot

## IIS Site
| | |
|-|-|
| **Host** | Windows Server 2025 |
| **Site** | telegram.socialmotive.net |
| **App pool** | telegram.socialmotive.nl |
| **Site root** | `c:\sites\telegram.socialmotive.net\` |
| **URL** | https://telegram.socialmotive.net |

## Prerequisites
- .NET 10 Hosting Bundle installed (includes ASP.NET Core Module v2)
- IIS app pool set to **No Managed Code**
- `ASPNETCORE_ENVIRONMENT` = `Production` set on the app pool (IIS Manager → App Pools → Advanced Settings → Environment Variables)

## Production Config (`appsettings.Production.json`)
File is included in the publish output at `c:\sites\telegram.socialmotive.net\appsettings.Production.json`.
Edit directly on the server after publishing, then recycle the app pool.

| Setting | Value |
|---------|-------|
| `TelegramBot:BotToken` | Bot token from @BotFather |
| `TelegramBot:Enabled` | `true` |
| `TelegramBot:UseWebhook` | `true` |
| `TelegramBot:WebhookUrl` | `https://telegram.socialmotive.net/api/telegram/webhook` |
| `TelegramBot:WebhookSecretToken` | A strong random secret (e.g. a GUID — generate once, never change) |
| `WebApiBaseUrl` | `https://telegram.socialmotive.net/` |
| `DataProtection:KeysPath` | `c:\sites\dataprotection-keys` |

> **WebhookSecretToken** is registered with Telegram automatically on app startup via `SetWebhook()`.
> If you change this value you must redeploy so the new secret is re-registered.

## Shared DataProtection Keys
`SocialMotive.WebApp` and `SocialMotive.TelegramBot` share a key ring so auth cookies issued by WebApp are accepted here.

- Directory: `c:\sites\dataprotection-keys\`
- The IIS app pool identity needs **Modify** permission on this folder
- Both apps use `SetApplicationName("SocialMotive")` — do not change this value

## Publish & Deploy

### 1. Stop the app pool (releases the DLL lock)
```
C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:"telegram.socialmotive.nl"
```

### 2. Publish from the repo root
```
dotnet publish src/SocialMotive.TelegramBot/SocialMotive.TelegramBot.csproj ^
  -c Release ^
  -o "c:\sites\telegram.socialmotive.net" ^
  --self-contained false
```

### 3. Start the app pool
```
C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:"telegram.socialmotive.nl"
```

## Webhook Registration
`TelegramBotService` calls `SetWebhook()` automatically on startup when `UseWebhook = true`.
No action required in BotFather — the webhook URL is registered in code.

To verify the webhook is active:
```
https://api.telegram.org/bot{TOKEN}/getWebhookInfo
```

## Verification
1. Browse `https://telegram.socialmotive.net/` — status page should show **Running** with bot username
2. Send `/status` to @SocialMotiveBot in Telegram — should receive a reply
3. Check Seq for `"Telegram webhook registered"` log entry

## Logs
Structured logs via Serilog → Seq at `http://localhost:5341` (configurable via `Seq:ServerUrl`).

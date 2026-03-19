# Deployment — SocialMotive.LiveMap

## IIS Site
| | |
|-|-|
| **Host** | Windows Server 2025 (same server as TelegramBot) |
| **Site** | livemap.socialmotive.net |
| **App pool** | livemap.socialmotive.net |
| **Site root** | `c:\sites\livemap.socialmotive.net\` |
| **URL** | https://livemap.socialmotive.net |

## Prerequisites
- .NET 10 Hosting Bundle installed (includes ASP.NET Core Module v2)
- IIS app pool set to **No Managed Code**
- `ASPNETCORE_ENVIRONMENT` = `Production` set on the app pool

## Production Config (`appsettings.Production.json`)
Edit directly on the server after publishing, then recycle the app pool.

| Setting | Value |
|---------|-------|
| `ConnectionStrings:SocialMotive` | Production SQL Server connection string |
| `GoogleMaps:ApiKey` | Google Maps API key (optional — map works without it with watermark) |

## Publish & Deploy

### 1. Stop the app pool
```
C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:"livemap.socialmotive.net"
```

### 2. Publish from the repo root
```
dotnet publish src/SocialMotive.LiveMap/SocialMotive.LiveMap.csproj ^
  -c Release ^
  -o "c:\sites\livemap.socialmotive.net" ^
  --self-contained false
```

### 3. Start the app pool
```
C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:"livemap.socialmotive.net"
```

## Verification
1. Browse `https://livemap.socialmotive.net/` — Leaflet map page should load
2. Navigate to `/google` and `/telerik` — each map variant should render
3. Send a live location from Telegram → marker should appear/update on all map pages

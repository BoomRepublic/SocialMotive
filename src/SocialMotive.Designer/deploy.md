# Deployment — SocialMotive.Designer

## IIS Site
| | |
|-|-|
| **Host** | Windows Server 2025 |
| **Site** | design.socialmotive.net |
| **App pool** | design.socialmotive.net |
| **Site root** | `c:\sites\design.socialmotive.net\` |
| **URL** | https://design.socialmotive.net |

## Prerequisites
- .NET 10 Hosting Bundle installed (includes ASP.NET Core Module v2)
- IIS app pool set to **No Managed Code**
- `ASPNETCORE_ENVIRONMENT` = `Production` set on the app pool

## Production Config (`appsettings.Production.json`)
Edit directly on the server after publishing, then recycle the app pool.

| Setting | Value |
|---------|-------|
| `ConnectionStrings:SocialMotive` | Production SQL Server connection string |
| `Seq:ServerUrl` | Seq server URL |
| `DataProtection:KeysPath` | Shared keys directory (same as WebApp and TelegramBot) |

## Publish & Deploy

### 1. Stop the app pool
```
C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:"design.socialmotive.net"
```

### 2. Publish from the repo root
```
dotnet publish src/SocialMotive.Designer/SocialMotive.Designer.csproj ^
  -c Release ^
  -o "c:\sites\design.socialmotive.net" ^
  --self-contained false
```

### 3. Start the app pool
```
C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:"design.socialmotive.net"
```

## Verification
1. Browse `https://design.socialmotive.net/` — Designer home page loads with dark Telerik theme
2. Browse `https://design.socialmotive.net/swagger` — Swagger UI shows DesignController endpoints
3. Check Seq for log entries from this app (source: SocialMotive.Designer)
4. Open browser dev tools and confirm SignalR websocket connects to `/designhub`

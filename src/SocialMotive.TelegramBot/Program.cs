using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SocialMotive.Core.Data;
using SocialMotive.Core.Mapping;
using SocialMotive.Core.Services;
using SocialMotive.TelegramBot.Services;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, lc) => lc
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(
            context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341",
            apiKey: context.Configuration["Seq:ApiKey"]));

    // Persist DataProtection keys to a stable directory so they survive IIS app pool recycles.
    // Both WebApp and TelegramBot use the same KeysPath + ApplicationName so cookies are mutually valid.
    var keysPath = builder.Configuration["DataProtection:KeysPath"]
        ?? Path.Combine(builder.Environment.ContentRootPath, "keys");
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName("SocialMotive");

    builder.Services.AddDbContext<SocialMotiveDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive")));

    builder.Services.AddAutoMapper(cfg =>
    {
        var license = builder.Configuration["Licenses:AutoMapper"];
        if (!string.IsNullOrEmpty(license))
            cfg.LicenseKey = license;
        cfg.AddMaps(typeof(AdminMappingProfile).Assembly);
    });

    builder.Services.Configure<TelegramBotSettings>(
        builder.Configuration.GetSection(TelegramBotSettings.SectionName));

    builder.Services.AddScoped<ITrackerService, TrackerService>();
    builder.Services.AddSingleton<BotStatusService>();
    builder.Services.AddSingleton<TelegramUpdateHandler>();
    builder.Services.AddHostedService<TelegramBotService>();

    // HttpClient to call WebApp's TelegramController endpoints (redeem-code)
    builder.Services.AddHttpClient("WebApi", client =>
        client.BaseAddress = new Uri(builder.Configuration["WebApiBaseUrl"] ?? "https://localhost:7050/"));

    // HttpClient to call LiveMap's callback endpoint for real-time map updates
    builder.Services.AddHttpClient("LiveMapApi", client =>
        client.BaseAddress = new Uri(builder.Configuration["LiveMapBaseUrl"] ?? "https://localhost:7052/"));

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
    builder.Services.AddAuthorization();

    builder.Services.AddControllers()
        .AddApplicationPart(typeof(SocialMotive.Core.Controllers.TelegramController).Assembly);

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapGet("/", (BotStatusService status) =>
    {
        var uptime = status.StartedAt.HasValue
            ? (DateTime.UtcNow - status.StartedAt.Value).ToString(@"d\d\ hh\:mm\:ss")
            : "-";
        var color = status.IsRunning ? "#22c55e" : "#ef4444";
        var label = status.IsRunning ? "Running" : "Stopped";
        var botName = status.BotUsername != null ? "@" + status.BotUsername : "-";
        var botId = status.BotId?.ToString() ?? "-";
        var startedAt = status.StartedAt?.ToString("yyyy-MM-dd HH:mm:ss") + " UTC" ?? "-";
        var errorRow = status.ErrorMessage != null
            ? $"<tr><td>Error</td><td style=\"color:#f87171\">{status.ErrorMessage}</td></tr>"
            : "";
        var bgAlpha = color + "22";
        var borderAlpha = color + "55";

        var html = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8"/>
              <meta name="viewport" content="width=device-width, initial-scale=1"/>
              <title>SocialMotive TelegramBot</title>
              <style>
                body { font-family: system-ui, sans-serif; background: #0f172a; color: #e2e8f0; margin: 0; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .card { background: #1e293b; border-radius: 12px; padding: 2.5rem 3rem; min-width: 360px; box-shadow: 0 4px 32px rgba(0,0,0,.4); }
                h1 { margin: 0 0 1.5rem; font-size: 1.4rem; color: #94a3b8; }
                .badge { display: inline-block; background: {{bgAlpha}}; color: {{color}}; border: 1px solid {{borderAlpha}}; border-radius: 999px; padding: .2rem .9rem; font-weight: 600; font-size: .95rem; margin-bottom: 1.5rem; }
                table { width: 100%; border-collapse: collapse; }
                td { padding: .45rem 0; font-size: .95rem; }
                td:first-child { color: #94a3b8; width: 50%; }
                td:last-child { color: #f1f5f9; font-weight: 500; }
              </style>
            </head>
            <body>
              <div class="card">
                <h1>SocialMotive TelegramBot</h1>
                <div class="badge">{{label}}</div>
                <table>
                  <tr><td>Bot</td><td>{{botName}}</td></tr>
                  <tr><td>Bot ID</td><td>{{botId}}</td></tr>
                  <tr><td>Started</td><td>{{startedAt}}</td></tr>
                  <tr><td>Uptime</td><td>{{uptime}}</td></tr>
                  {{errorRow}}
                </table>
              </div>
            </body>
            </html>
            """;
        return Results.Content(html, "text/html");
    }).AllowAnonymous();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "TelegramBot terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

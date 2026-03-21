using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SocialMotive.Core.Controllers;
using SocialMotive.Core.Data;
using SocialMotive.Core.Mapping;
using SocialMotive.Core.Services;
using SocialMotive.Designer.Components;
using SocialMotive.Designer.Hubs;
using SocialMotive.Designer.Services;

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

    var keysPath = builder.Configuration["DataProtection:KeysPath"]
        ?? Path.Combine(builder.Environment.ContentRootPath, "keys");
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName("SocialMotive");

    var sqlTimeout = builder.Configuration.GetValue<int>("Database:CommandTimeoutSeconds", 120);

    builder.Services.AddDbContext<SocialMotiveDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive"),
            sql => sql.CommandTimeout(sqlTimeout)));

    builder.Services.AddDbContextFactory<SocialMotiveDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive"),
            sql => sql.CommandTimeout(sqlTimeout)),
        ServiceLifetime.Scoped);

    builder.Services.AddAutoMapper(cfg =>
    {
        var license = builder.Configuration["Licenses:AutoMapper"];
        if (!string.IsNullOrEmpty(license))
            cfg.LicenseKey = license;
        cfg.AddMaps(typeof(AdminMappingProfile).Assembly);
    });

    builder.Services.AddHttpClient("Auth", client =>
    {
        var baseUrl = builder.Configuration["BaseUrl"] ?? "https://localhost:7053";
        client.BaseAddress = new Uri(baseUrl);
    });

    builder.Services.AddScoped<IDesignService, DesignService>();
    builder.Services.AddSingleton<IBackgroundRenderQueue, BackgroundRenderQueue>();
    builder.Services.AddHostedService<RenderWorkerService>();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/access-denied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();

    builder.Services.AddTelerikBlazor();

    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        options.SwaggerDoc("v1", new()
        {
            Title = "SocialMotive Designer API",
            Version = "v1",
            Description = "REST API for the SocialMotive Designer feature area."
        });

        var coreXmlFile = $"{typeof(DesignController).Assembly.GetName().Name}.xml";
        var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
        if (File.Exists(coreXmlPath))
            options.IncludeXmlComments(coreXmlPath);
    });

    builder.Services.AddSignalR();

    builder.Services.AddControllers()
        .AddApplicationPart(typeof(DesignController).Assembly);

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialMotive Designer API v1");
        options.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSerilogRequestLogging();
    app.UseAntiforgery();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<DesignHub>("/designhub");
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Designer terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

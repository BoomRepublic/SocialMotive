using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;
using SocialMotive.Core.Mapping;
using SocialMotive.WebApp.Components;
using SocialMotive.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelerikBlazor();

// Add HttpContextAccessor for cookie forwarding
builder.Services.AddHttpContextAccessor();

// Add HttpClient factories for API calls from Blazor components
var apiBaseUrl = (builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7050").TrimEnd('/');

builder.Services.AddTransient<CookieForwardingHandler>();

builder.Services.AddHttpClient("AdminApi", client =>
    client.BaseAddress = new Uri($"{apiBaseUrl}/api/admin/"))
    .AddHttpMessageHandler<CookieForwardingHandler>();
builder.Services.AddScoped<AdminApiService>();

builder.Services.AddHttpClient("GeneratorApi", client =>
    client.BaseAddress = new Uri($"{apiBaseUrl}/api/generator/"))
    .AddHttpMessageHandler<CookieForwardingHandler>();
builder.Services.AddScoped<GeneratorApiService>();

builder.Services.AddHttpClient("PublicApi", client =>
    client.BaseAddress = new Uri($"{apiBaseUrl}/api/"));
builder.Services.AddScoped<PublicApiService>();

builder.Services.AddHttpClient("VolunteerApi", client =>
    client.BaseAddress = new Uri($"{apiBaseUrl}/api/volunteer/"))
    .AddHttpMessageHandler<CookieForwardingHandler>();
builder.Services.AddScoped<VolunteerApiService>();

// Add DbContext for SocialMotive database
builder.Services.AddDbContext<SocialMotiveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive")));

// Add AutoMapper profiles from Core assembly
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AdminMappingProfile).Assembly));

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/demo-logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("CanAccessGenerator", policy => policy.RequireRole(AppRoles.AssetManager, AppRoles.Admin));
    options.AddPolicy("CanAccessWeb", policy => policy.RequireRole(AppRoles.Volunteer, AppRoles.Organizer, AppRoles.Admin));
});

// Add cascading authentication state for Blazor components
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(SocialMotive.Core.Controllers.GeneratorController).Assembly);

// Add Swagger/OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new()
    {
        Title = "SocialMotive Admin API",
        Version = "v1",
        Description = "REST API for managing SocialMotive database tables (Users, Trackers, Events, Labels, EventTypes)",
        Contact = new()
        {
            Name = "SocialMotive Admin",
            Email = "admin@socialmotive.local"
        }
    });

    // Include XML comments from WebApp and Core assemblies
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    var coreXmlFile = $"{typeof(SocialMotive.Core.Controllers.GeneratorController).Assembly.GetName().Name}.xml";
    var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
    if (File.Exists(coreXmlPath))
        options.IncludeXmlComments(coreXmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger in development and always for API documentation
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialMotive Admin API v1");
    options.RoutePrefix = "swagger";
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

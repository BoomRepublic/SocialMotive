using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SocialMotive.WebApp;
using SocialMotive.WebApp.Components;
using SocialMotive.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelerikBlazor();

// Add DbContext for SocialMotive database
builder.Services.AddDbContext<SocialMotiveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive")));

// Add authentication (claims-based)
builder.Services.AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthenticationHandler>("Bearer", _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminFull", policy => policy.RequireRole("AdminFull"));
    options.AddPolicy("CanAccessAdmin", policy => policy.RequireRole("AdminFull"));
    options.AddPolicy("CanAccessGenerator", policy => policy.RequireRole("Admin", "AdminFull"));
    options.AddPolicy("CanAccessWeb", policy => policy.RequireRole("Volunteer", "Organizer", "Admin", "AdminFull"));
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// Add Swagger/OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
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

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
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

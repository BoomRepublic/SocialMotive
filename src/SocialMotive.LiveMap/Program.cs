using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Data;
using SocialMotive.Core.Services;
using SocialMotive.LiveMap.Components;
using SocialMotive.LiveMap.Hubs;
using SocialMotive.LiveMap.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SocialMotiveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialMotive")));

builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddSingleton<LocationCacheService>();
builder.Services.AddScoped<ITrackerService, TrackerService>();
builder.Services.AddHostedService<TrackerTimeoutService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapHub<LocationHub>("/locationhub");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

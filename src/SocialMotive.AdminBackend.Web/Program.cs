using SocialMotive.AdminBackend.Web.Components;
using SocialMotive.AdminBackend.Web.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration["Oidc:Authority"] ?? "https://auth.example.com";
    options.ClientId = builder.Configuration["Oidc:ClientId"] ?? "socialmotive-admin";
    options.ClientSecret = builder.Configuration["Oidc:ClientSecret"] ?? "dev-secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("openid");
});

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

// Add API services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://api.admin.socialmotive.local") });
builder.Services.AddScoped<IMetadataApiService, MetadataApiService>();
builder.Services.AddScoped<ITableCrudApiService, TableCrudApiService>();
builder.Services.AddScoped<IExportApiService, ExportApiService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

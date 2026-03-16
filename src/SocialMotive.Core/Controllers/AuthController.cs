using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Data;

namespace SocialMotive.Core.Controllers;

/// <summary>
/// Authentication Controller for demo login/logout.
/// Sets up ClaimsPrincipal with UserId, Name, Email, and Role claims.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly SocialMotiveDbContext _dbContext;

    public AuthController(
        ILogger<AuthController> logger,
        SocialMotiveDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Demo login endpoint — signs in as a demo user with the specified role.
    /// Looks up the matching demo user by email and creates a ClaimsPrincipal
    /// with UserId, Name, Email, and Role claims.
    /// </summary>
    [HttpGet("demo-login/{role}")]
    public async Task<IActionResult> DemoLogin(string role, [FromQuery] string? returnUrl = "/")
    {
        try
        {
            // Map role parameter to authorization role and demo email
            var (authRole, demoEmail) = role.ToLowerInvariant() switch
            {
                "admin" => (AppRoles.Admin, "admin.demo@socialmotive.net"),
                "organizer" => (AppRoles.Organizer, "organizer.demo@socialmotive.net"),
                "volunteer" => (AppRoles.Volunteer, "volunteer.demo@socialmotive.net"),
                _ => (AppRoles.Volunteer, "volunteer.demo@socialmotive.net")
            };

            // Look up the demo user by email
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == demoEmail);

            if (user == null)
            {
                _logger.LogWarning("Demo user not found for email {Email}", demoEmail);
                return Redirect("/login?error=no-users");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // Load roles from UserRoles → Roles table
            var roleNames = await _dbContext.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_dbContext.Roles,
                    ur => ur.RoleId,
                    r => r.RoleId,
                    (ur, r) => r.Name)
                .Where(name => name != null)
                .ToListAsync();

            // If no roles assigned, fall back to the demo role parameter
            if (roleNames.Count == 0)
            {
                roleNames.Add(authRole);
            }

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName!));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            _logger.LogInformation("Demo login: UserId={UserId}, Role={Role}", user.UserId, authRole);

            return Redirect(returnUrl ?? "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during demo login");
            return Redirect("/login?error=login-failed");
        }
    }

    /// <summary>
    /// Demo logout endpoint — signs out and redirects to login page
    /// </summary>
    [HttpGet("demo-logout")]
    public async Task<IActionResult> DemoLogout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}

using Microsoft.AspNetCore.Http;

namespace SocialMotive.Core.Services;

/// <summary>
/// Delegating handler that forwards incoming auth cookies to outbound API requests.
/// Add to any named HttpClient that calls authenticated endpoints.
/// </summary>
public class CookieForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.Request.Cookies.Count > 0)
        {
            var cookieHeader = string.Join("; ", context.Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", cookieHeader);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

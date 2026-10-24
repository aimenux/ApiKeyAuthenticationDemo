using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net;

namespace Example02.Presentation.Authentication;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<ApiKeyOptions> _options;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Api key is missing");
            return;
        }

        var actualApiKey = _options.Value.ApiKey;
        
        if (!IsValidApiKey(requestApiKey.ToString(), actualApiKey))
        {
            context.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Api key is invalid");
            return;
        }

        await _next(context);
    }

    private static bool IsValidApiKey(string requestApiKey, string actualApiKey)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(requestApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(actualApiKey));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }
}
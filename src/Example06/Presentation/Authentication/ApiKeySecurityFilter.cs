using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Example06.Presentation.Authentication;

public class ApiKeySecurityFilter : IEndpointFilter
{
    private readonly IOptions<ApiKeyOptions> _options;

    public ApiKeySecurityFilter(IOptions<ApiKeyOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            return HttpResults.Unauthorized("Api key is missing");
        }

        var actualApiKey = _options.Value.ApiKey;
        
        if (!IsValidApiKey(requestApiKey.ToString(), actualApiKey))
        {
            return HttpResults.Unauthorized("Api key is invalid");
        }

        return await next(context);
    }

    private static bool IsValidApiKey(string requestApiKey, string actualApiKey)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(requestApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(actualApiKey));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}
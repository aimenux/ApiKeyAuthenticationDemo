using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Example07.Presentation.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IOptions<ApiKeyOptions> _apiKeyOptions;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<ApiKeyOptions> apiKeyOptions) : base(options,
        logger,
        encoder)
    {
        _apiKeyOptions = apiKeyOptions;
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
        return base.HandleChallengeAsync(properties);
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Api key is missing"));
        }

        var actualApiKey = _apiKeyOptions.Value.ApiKey;
        
        if (!IsValidApiKey(requestApiKey.ToString(), actualApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Api key is invalid"));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, ApiKeyConstants.ApiKeyOwner)
        };
        var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Options.AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool IsValidApiKey(string requestApiKey, string actualApiKey)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(requestApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(actualApiKey));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}
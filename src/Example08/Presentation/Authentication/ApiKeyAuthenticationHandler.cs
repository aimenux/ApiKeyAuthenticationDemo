using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Example08.Presentation.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options,
        logger,
        encoder,
        clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Api key is missing"));
        }

        var configuration = Context.RequestServices.GetRequiredService<IConfiguration>();
        var actualApiKey = configuration.GetValue<string>(ApiKeyConstants.ApiKeySectionName);
        if (requestApiKey != actualApiKey)
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
}
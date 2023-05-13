using Microsoft.AspNetCore.Authentication;

namespace Example07.Presentation.Authentication;

public static class SecurityExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = ApiKeyConstants.ApiKeyScheme;
                options.DefaultChallengeScheme = ApiKeyConstants.ApiKeyScheme;
                options.DefaultAuthenticateScheme = ApiKeyConstants.ApiKeyScheme;
            })
            .AddApiKeyScheme(options =>
            {
                options.AuthenticationScheme = ApiKeyConstants.ApiKeyScheme;
                options.AuthenticationType = ApiKeyConstants.ApiKeyScheme;
            });

        return services;
    }
    
    private static AuthenticationBuilder AddApiKeyScheme(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
    {
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyConstants.ApiKeyScheme, options);
    }
}
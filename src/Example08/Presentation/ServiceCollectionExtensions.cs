using Example08.Presentation.Authentication;
using Example08.Presentation.Endpoints;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example08.Presentation;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<ApiKeyOptions>()
            .BindConfiguration(ApiKeyOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), "Authentication:ApiKey must be set to a non-empty API key.")
            .ValidateOnStart();

        builder.Services.AddEndpoints();
        builder.Services.AddSwagger();
        builder.Services.AddApiKeyAuthentication();
        builder.Services.AddAuthorization();
        return builder;
    }

    private static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddScoped<IMoviesEndpoints, MoviesEndpoints>();
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}

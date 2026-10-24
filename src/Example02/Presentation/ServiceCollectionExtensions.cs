using Example02.Presentation.Endpoints;
using Microsoft.Extensions.Options;
using Example02.Presentation.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example02.Presentation;

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

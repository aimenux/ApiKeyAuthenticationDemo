using Example05.Presentation.Authentication;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example05.Presentation;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services
            .AddOptions<ApiKeyOptions>()
            .BindConfiguration(ApiKeyOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), "Authentication:ApiKey must be set to a non-empty API key.")
            .ValidateOnStart();

        builder.Services.AddControllers();
        builder.Services.AddSwagger();
        return builder;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddScoped<ApiKeySecurityFilter>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}

using Example02.Presentation.Authentication;
using Example02.Presentation.Endpoints;
using Microsoft.OpenApi.Models;

namespace Example02.Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddScoped<IMoviesEndpoints, MoviesEndpoints>();
        return services;
    }
    
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(ApiKeyConstants.ApiKeyId, new OpenApiSecurityScheme
            {
                Description = "The Api Key to access the API",
                Name = ApiKeyConstants.ApiKeyHeaderName,
                Scheme = ApiKeyConstants.ApiKeyScheme,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = ApiKeyConstants.ApiKeyId,
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}
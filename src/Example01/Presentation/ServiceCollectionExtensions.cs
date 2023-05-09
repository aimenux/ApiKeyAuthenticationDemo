using Example01.Presentation.Authentication;
using Microsoft.OpenApi.Models;

namespace Example01.Presentation;

public static class ServiceCollectionExtensions
{
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
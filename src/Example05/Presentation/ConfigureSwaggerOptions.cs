using Example05.Presentation.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example05.Presentation;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
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
    }
}
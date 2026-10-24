using Example08.Presentation.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example08.Presentation;

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

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyConstants.ApiKeyId, document)] = []
        });
    }
}
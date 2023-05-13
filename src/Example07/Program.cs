using Example07.Infrastructure;
using Example07.Presentation;
using Example07.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure();
builder.Services.AddSwagger();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyConstants.ApiKeyScheme;
    options.DefaultChallengeScheme = ApiKeyConstants.ApiKeyScheme;
}).AddApiKeyAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using Example01.Infrastructure;
using Example01.Presentation;
using Example01.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseApiKeyAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
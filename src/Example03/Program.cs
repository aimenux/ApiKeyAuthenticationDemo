using Example03.Infrastructure;
using Example03.Presentation;
using Example03.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(x => x.Filters.Add<ApiKeyFilter>());
builder.Services.AddInfrastructure();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
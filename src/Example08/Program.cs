using Example08.Infrastructure;
using Example08.Presentation;
using Example08.Presentation.Authentication;
using Example08.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddInfrastructure();
builder.Services.AddSwagger();

builder.Services.AddApiKeyAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var appGroup = app
    .MapGroup("/api/movies")
    .RequireAuthorization()
    .WithGroupName("Movies");

appGroup
    .MapGet("list", (IMoviesEndpoints endpoints, CancellationToken cancellationToken) => endpoints.GetMoviesAsync(cancellationToken))
    .WithName("GetMovies");

appGroup
    .MapGet("{movieId:int}", (IMoviesEndpoints endpoints, int movieId, CancellationToken cancellationToken) => endpoints.GetMovieByIdAsync(movieId, cancellationToken))
    .WithName("GetMovieById");

app.Run();
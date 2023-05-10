using Example06.Infrastructure;
using Example06.Presentation;
using Example06.Presentation.Authentication;
using Example06.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddInfrastructure();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app
    .MapGet("/api/movies/list", (IMoviesEndpoints endpoints, CancellationToken cancellationToken) => endpoints.GetMoviesAsync(cancellationToken))
    .AddEndpointFilter<ApiKeySecurityFilter>()
    .WithName("GetMovies");

app
    .MapGet("/api/movies/{movieId:int}", (IMoviesEndpoints endpoints, int movieId, CancellationToken cancellationToken) => endpoints.GetMovieByIdAsync(movieId, cancellationToken))
    .AddEndpointFilter<ApiKeySecurityFilter>()
    .WithName("GetMovieById");

app.Run();
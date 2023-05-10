using Example04.Infrastructure;
using Example04.Presentation;
using Example04.Presentation.Authentication;
using Example04.Presentation.Endpoints;

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
    .AddEndpointFilter<ApiKeyFilter>()
    .WithName("GetMovies");

app
    .MapGet("/api/movies/{movieId:int}", (IMoviesEndpoints endpoints, int movieId, CancellationToken cancellationToken) => endpoints.GetMovieByIdAsync(movieId, cancellationToken))
    .AddEndpointFilter<ApiKeyFilter>()
    .WithName("GetMovieById");

app.Run();
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

var appGroup = app
    .MapGroup("/api/movies")
    .AddEndpointFilter<ApiKeySecurityFilter>()
    .WithGroupName("Movies");

appGroup
    .MapGet("list", (IMoviesEndpoints endpoints, CancellationToken cancellationToken) => endpoints.GetMoviesAsync(cancellationToken))
    .WithName("GetMovies");

appGroup
    .MapGet("{movieId:int}", (IMoviesEndpoints endpoints, int movieId, CancellationToken cancellationToken) => endpoints.GetMovieByIdAsync(movieId, cancellationToken))
    .WithName("GetMovieById");

app.Run();
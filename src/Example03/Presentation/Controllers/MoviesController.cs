using Example03.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Example03.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _repository;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieRepository repository, ILogger<MoviesController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetMoviesAsync(CancellationToken cancellationToken)
    {
        var movies = await _repository.GetMoviesAsync(cancellationToken);
        return Ok(movies);
    }
    
    [HttpGet("{movieId:int}")]
    public async Task<IActionResult> GetMovieByIdAsync([FromRoute] int movieId, CancellationToken cancellationToken)
    {
        var movie = await _repository.GetMovieByIdAsync(movieId, cancellationToken);
        return movie is null ? NotFound() : Ok(movie);
    }
}
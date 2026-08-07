using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class MoviesController : ControllerBase
{
    private const string GetMovieByIdRoute = "GetMovieById";

    private static readonly HashSet<string> AllowedPosterExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png"
    };

    private const long MaxPosterSize = 1024 * 1024;

    private readonly IMoviesService _moviesService;
    private readonly IGenresService _genresService;

    public MoviesController(IMoviesService moviesService, IGenresService genresService)
    {
        _moviesService = moviesService;
        _genresService = genresService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MovieDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MovieDetailsDto>>> GetAllAsync(
        [FromQuery] MovieQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var result = await _moviesService.GetPageAsync(queryParameters, cancellationToken);

        return Ok(new PagedResult<MovieDetailsDto>
        {
            Items = result.Items.Select(movie => movie.ToDetailsDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}", Name = GetMovieByIdRoute)]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var movie = await _moviesService.GetByIdAsync(id, cancellationToken);
        return movie is null ? NotFound() : Ok(movie.ToDetailsDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDetailsDto>> CreateAsync(
        [FromForm] MovieDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Poster is null)
            return BadRequest(new { message = "Poster is required." });

        var posterValidationError = ValidatePoster(dto.Poster);
        if (posterValidationError is not null)
            return BadRequest(new { message = posterValidationError });

        var genre = await _genresService.GetByIdAsync(dto.GenreId, cancellationToken);
        if (genre is null)
            return BadRequest(new { message = "Invalid genre ID." });

        await using var stream = new MemoryStream();
        await dto.Poster.CopyToAsync(stream, cancellationToken);

        var movie = new Movie
        {
            Title = dto.Title.Trim(),
            Year = dto.Year,
            Rate = dto.Rate,
            Storeline = dto.Storeline.Trim(),
            Poster = stream.ToArray(),
            GenreId = dto.GenreId,
            Genre = genre
        };

        await _moviesService.AddAsync(movie, cancellationToken);

        var response = movie.ToDetailsDto();
        return CreatedAtRoute(GetMovieByIdRoute, new { id = movie.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> UpdateAsync(
        int id,
        [FromForm] MovieDto dto,
        CancellationToken cancellationToken)
    {
        var movie = await _moviesService.GetByIdAsync(id, cancellationToken);
        if (movie is null)
            return NotFound();

        var genre = await _genresService.GetByIdAsync(dto.GenreId, cancellationToken);
        if (genre is null)
            return BadRequest(new { message = "Invalid genre ID." });

        if (dto.Poster is not null)
        {
            var posterValidationError = ValidatePoster(dto.Poster);
            if (posterValidationError is not null)
                return BadRequest(new { message = posterValidationError });

            await using var stream = new MemoryStream();
            await dto.Poster.CopyToAsync(stream, cancellationToken);
            movie.Poster = stream.ToArray();
        }

        movie.Title = dto.Title.Trim();
        movie.Year = dto.Year;
        movie.Rate = dto.Rate;
        movie.Storeline = dto.Storeline.Trim();
        movie.GenreId = dto.GenreId;
        movie.Genre = genre;

        await _moviesService.UpdateAsync(movie, cancellationToken);
        return Ok(movie.ToDetailsDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var movie = await _moviesService.GetByIdAsync(id, cancellationToken);
        if (movie is null)
            return NotFound();

        await _moviesService.DeleteAsync(movie, cancellationToken);
        return NoContent();
    }

    private static string? ValidatePoster(IFormFile poster)
    {
        var extension = Path.GetExtension(poster.FileName);

        if (!AllowedPosterExtensions.Contains(extension))
            return "Only .jpg, .jpeg, and .png images are allowed.";

        if (poster.Length <= 0)
            return "Poster file is empty.";

        if (poster.Length > MaxPosterSize)
            return "Maximum allowed poster size is 1 MB.";

        return null;
    }
}

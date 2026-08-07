using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class GenresController : ControllerBase
{
    private readonly IGenresService _genresService;

    public GenresController(IGenresService genresService)
    {
        _genresService = genresService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenreDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenreDetailsDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var genres = await _genresService.GetAllAsync(cancellationToken);
        return Ok(genres.Select(genre => new GenreDetailsDto { Id = genre.Id, Name = genre.Name }));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GenreDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDetailsDto>> GetByIdAsync(byte id, CancellationToken cancellationToken)
    {
        var genre = await _genresService.GetByIdAsync(id, cancellationToken);
        return genre is null
            ? NotFound()
            : Ok(new GenreDetailsDto { Id = genre.Id, Name = genre.Name });
    }

    [HttpPost]
    [ProducesResponseType(typeof(GenreDetailsDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<GenreDetailsDto>> CreateAsync(GenreDto dto, CancellationToken cancellationToken)
    {
        var genre = new Genre { Name = dto.Name.Trim() };
        await _genresService.AddAsync(genre, cancellationToken);

        var response = new GenreDetailsDto { Id = genre.Id, Name = genre.Name };
        return CreatedAtAction(nameof(GetByIdAsync), new { id = genre.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GenreDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDetailsDto>> UpdateAsync(
        byte id,
        GenreDto dto,
        CancellationToken cancellationToken)
    {
        var genre = await _genresService.GetByIdAsync(id, cancellationToken);
        if (genre is null)
            return NotFound();

        genre.Name = dto.Name.Trim();
        await _genresService.UpdateAsync(genre, cancellationToken);

        return Ok(new GenreDetailsDto { Id = genre.Id, Name = genre.Name });
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(byte id, CancellationToken cancellationToken)
    {
        var genre = await _genresService.GetByIdAsync(id, cancellationToken);
        if (genre is null)
            return NotFound();

        await _genresService.DeleteAsync(genre, cancellationToken);
        return NoContent();
    }
}

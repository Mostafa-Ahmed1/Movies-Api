namespace MoviesAPI.Dtos;

public sealed class MovieDto
{
    [Required, MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; set; }

    [Range(0, 10)]
    public double Rate { get; set; }

    [Required, MaxLength(2500)]
    public string Storeline { get; set; } = string.Empty;

    public IFormFile? Poster { get; set; }

    [Range(1, byte.MaxValue)]
    public byte GenreId { get; set; }
}

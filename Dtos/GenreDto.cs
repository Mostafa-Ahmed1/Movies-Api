namespace MoviesAPI.Dtos;

public sealed class GenreDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

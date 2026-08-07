namespace MoviesAPI.Dtos;

public sealed class MovieDetailsDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Year { get; init; }
    public double Rate { get; init; }
    public string Storeline { get; init; } = string.Empty;
    public byte[] Poster { get; init; } = Array.Empty<byte>();
    public byte GenreId { get; init; }
    public string GenreName { get; init; } = string.Empty;
}

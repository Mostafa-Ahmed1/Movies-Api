namespace MoviesAPI.Helpers;

public static class DtoMappings
{
    public static MovieDetailsDto ToDetailsDto(this Movie movie)
    {
        return new MovieDetailsDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Rate = movie.Rate,
            Storeline = movie.Storeline,
            Poster = movie.Poster,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.Name ?? string.Empty
        };
    }
}

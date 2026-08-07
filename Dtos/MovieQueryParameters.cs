namespace MoviesAPI.Dtos;

public enum MovieSortBy
{
    Rate,
    Title,
    Year
}

public enum SortDirection
{
    Asc,
    Desc
}

public sealed class MovieQueryParameters
{
    public byte? GenreId { get; init; }

    [MaxLength(100)]
    public string? Search { get; init; }

    public MovieSortBy SortBy { get; init; } = MovieSortBy.Rate;

    public SortDirection SortDirection { get; init; } = SortDirection.Desc;

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 50)]
    public int PageSize { get; init; } = 10;
}

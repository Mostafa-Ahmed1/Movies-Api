namespace MoviesAPI.Services;

public sealed class MoviesService : IMoviesService
{
    private readonly ApplicationDbContext _db;

    public MoviesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Movie>> GetPageAsync(
        MovieQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Movies
            .AsNoTracking()
            .Include(movie => movie.Genre)
            .AsQueryable();

        if (queryParameters.GenreId.HasValue)
            query = query.Where(movie => movie.GenreId == queryParameters.GenreId.Value);

        if (!string.IsNullOrWhiteSpace(queryParameters.Search))
        {
            var search = queryParameters.Search.Trim();
            query = query.Where(movie =>
                movie.Title.Contains(search) || movie.Storeline.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (queryParameters.SortBy, queryParameters.SortDirection) switch
        {
            (MovieSortBy.Title, SortDirection.Asc) => query.OrderBy(movie => movie.Title).ThenBy(movie => movie.Id),
            (MovieSortBy.Title, SortDirection.Desc) => query.OrderByDescending(movie => movie.Title).ThenBy(movie => movie.Id),
            (MovieSortBy.Year, SortDirection.Asc) => query.OrderBy(movie => movie.Year).ThenBy(movie => movie.Id),
            (MovieSortBy.Year, SortDirection.Desc) => query.OrderByDescending(movie => movie.Year).ThenBy(movie => movie.Id),
            (MovieSortBy.Rate, SortDirection.Asc) => query.OrderBy(movie => movie.Rate).ThenBy(movie => movie.Id),
            _ => query.OrderByDescending(movie => movie.Rate).ThenBy(movie => movie.Id)
        };

        var items = await query
            .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Movie>
        {
            Items = items,
            Page = queryParameters.Page,
            PageSize = queryParameters.PageSize,
            TotalCount = totalCount
        };
    }

    public Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Movies
            .Include(movie => movie.Genre)
            .SingleOrDefaultAsync(movie => movie.Id == id, cancellationToken);
    }

    public async Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _db.Movies.AddAsync(movie, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return movie;
    }

    public async Task UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _db.Movies.Update(movie);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

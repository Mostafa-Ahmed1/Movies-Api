namespace MoviesAPI.Services;

public sealed class MoviesService : IMoviesService
{
    private readonly ApplicationDbContext _db;

    public MoviesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Movie>> GetAllAsync(byte? genreId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Movies
            .AsNoTracking()
            .Include(movie => movie.Genre)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(movie => movie.GenreId == genreId.Value);

        return await query
            .OrderByDescending(movie => movie.Rate)
            .ThenBy(movie => movie.Title)
            .ToListAsync(cancellationToken);
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

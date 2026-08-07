namespace MoviesAPI.Services;

public sealed class GenresService : IGenresService
{
    private readonly ApplicationDbContext _db;

    public GenresService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Genre?> GetByIdAsync(byte id, CancellationToken cancellationToken = default)
    {
        return _db.Genres.SingleOrDefaultAsync(genre => genre.Id == id, cancellationToken);
    }

    public async Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        await _db.Genres.AddAsync(genre, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return genre;
    }

    public async Task UpdateAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        _db.Genres.Update(genre);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(byte id, CancellationToken cancellationToken = default)
    {
        return _db.Genres.AnyAsync(genre => genre.Id == id, cancellationToken);
    }
}

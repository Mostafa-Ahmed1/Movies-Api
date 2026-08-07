namespace MoviesAPI.Services;

public interface IGenresService
{
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Genre?> GetByIdAsync(byte id, CancellationToken cancellationToken = default);
    Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default);
    Task UpdateAsync(Genre genre, CancellationToken cancellationToken = default);
    Task DeleteAsync(Genre genre, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(byte id, CancellationToken cancellationToken = default);
}

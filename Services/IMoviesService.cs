namespace MoviesAPI.Services;

public interface IMoviesService
{
    Task<IReadOnlyList<Movie>> GetAllAsync(byte? genreId = null, CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default);
    Task UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task DeleteAsync(Movie movie, CancellationToken cancellationToken = default);
}

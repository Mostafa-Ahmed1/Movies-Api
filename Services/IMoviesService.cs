namespace MoviesAPI.Services;

public interface IMoviesService
{
    Task<PagedResult<Movie>> GetPageAsync(MovieQueryParameters queryParameters, CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default);
    Task UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task DeleteAsync(Movie movie, CancellationToken cancellationToken = default);
}

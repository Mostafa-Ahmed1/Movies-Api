namespace MoviesAPI.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext db;

        public MoviesService(ApplicationDbContext db)
        {
            this.db=db;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            return await db.Movies
                .Where(m=>m.GenreId == genreId || genreId == 0)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await db.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id==id);
        }

        public async Task<Movie> Add(Movie movie)
        {
            await db.AddAsync(movie);
            db.SaveChanges();

            return movie;
        }

        public Movie Update(Movie movie)
        {
            db.Update(movie);
            db.SaveChanges();

            return movie;
        }

        public Movie Delete(Movie movie)
        {
            db.Remove(movie);
            db.SaveChanges();

            return movie;
        }
    }
}

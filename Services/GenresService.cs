namespace MoviesAPI.Services
{
    public class GenresService : IGenresService
    {
        private readonly ApplicationDbContext db;

        public GenresService(ApplicationDbContext db)
        {
            this.db=db;
        }

        public async Task<Genre> Add(Genre genre)
        {
            await db.AddAsync(genre);
            db.SaveChanges();

            return genre;
        }

        public Genre Delete(Genre genre)
        {
            db.Remove(genre);
            db.SaveChanges();

            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await db.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre> GetById(byte id)
        {
            return await db.Genres.SingleOrDefaultAsync(g => g.Id == id);
        }

        public Task<bool> isValidGenre(byte id)
        {
            return db.Genres.AnyAsync(g => g.Id == id);
        }

        public Genre Update(Genre genre)
        {
            db.Update(genre);
            db.SaveChanges();

            return genre;
        }
    }
}

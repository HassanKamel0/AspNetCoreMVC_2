using WoodyMovie.Models;

namespace WoodyMovie.Repository.IRepository
{
    public interface IMovieRepository
    {
        IQueryable<Movie> GetAll();
        IQueryable<Movie> GetBy(int id);
        IQueryable<Movie> Search(string name);
        void Create(Movie movie);
        void Update(Movie model);
        Task<Movie> FindAsync(int id);
        void Delete(int Id);
        void Save();
    }
}

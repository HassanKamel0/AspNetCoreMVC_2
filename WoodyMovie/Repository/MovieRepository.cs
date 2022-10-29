using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WoodyMovie.Data;
using WoodyMovie.Models;
using WoodyMovie.Repository.IRepository;

namespace WoodyMovie.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public MovieRepository(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }
        public void Create(Movie movie)
        {
            var mappedObj = mapper.Map<Movie>(movie);
            db.Movies.Add(mappedObj);
            db.SaveChanges();
        }

        public void Delete(int Id)
        {
            var movie = db.Movies.FirstOrDefault(p => p.Id == Id);
            if (movie != null)
                db.Movies.Remove(movie);
        }

        public async Task<Movie> FindAsync(int id)
        {
            var movie =await db.Movies.FindAsync(id);
            if (movie != null)
            {
                return mapper.Map<Movie>(movie);
            }
            return null;
        }

        public IQueryable<Movie> GetAll()
        {
            return db.Movies.OrderBy(u => u.Name);
        }

        public IQueryable<Movie> GetBy(int id)
        {
            return db.Movies.Where(u => u.Id == id);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public IQueryable<Movie> Search(string name)
        {
            var movies = db.Movies.Where(p => p.Name.Contains(name)).OrderBy(p => p.Name);
            return movies;
        }

        public void Update(Movie model)
        {
            var movie = db.Movies.FirstOrDefault(u => u.Id == model.Id);
            if (movie != null)
            {
                movie.Name = model.Name;
                movie.Category = model.Category;
                movie.CreatedDate = model.CreatedDate;
                movie.MovieSize = model.MovieSize;
                movie.DownloadCount = model.DownloadCount;
                if (model.ImageUrl != null)
                    movie.ImageUrl = model.ImageUrl;
            }
        }
    }
}

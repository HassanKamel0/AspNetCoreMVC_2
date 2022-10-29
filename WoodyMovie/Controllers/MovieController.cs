using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WoodyMovie.Data;
using WoodyMovie.Models;
using WoodyMovie.Repository.IRepository;

namespace WoodyMovie.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieRepository Imovie;
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hostEnvironment;

        public MovieController(IMovieRepository Imovie,ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            this.Imovie = Imovie;
            this.db = db;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View(Imovie.GetAll());
        }
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            Movie movie = new();
            if (id == null || id == 0)
            {
                return View(movie);   //create new
            }
            else
            {
                movie = await Imovie.FindAsync(id);        //update
                return View(movie);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile? file)
        {
			if (ModelState.IsValid)
			{
                string wwwRootPath = hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\movies");
                    var extension = Path.GetExtension(file.FileName);
                    if (movie.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, movie.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    movie.ImageUrl = @"\images\movies\" + fileName + extension;
                }
                if (movie.Id == 0)
                {
                    Imovie.Create(movie);
                    TempData["success"] = "Movie created successfully";
                }
                else
                {
                    Imovie.Update(movie);
                    TempData["success"] = "Movie updated successfully";
                }
                Imovie.Save();
                return RedirectToAction("Index");
			}
			return View(movie);
		}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await Imovie.FindAsync(id);
            if (movie == null)
                return NotFound();
            else
                return View(movie);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var movie = await Imovie.FindAsync(id);
            if (movie == null)
                return NotFound();
            Imovie.Delete(id);
            TempData["success"] = "Movie deleted successfully";
            Imovie.Save();
            return RedirectToAction("Index");
        }
        //[HttpGet]
        //[ValidateAntiForgeryToken]
        //public IActionResult Download(int id)
        //{
        //    var movie = db.Movies.Find(id);
        //    if (movie == null)
        //        return NotFound();
        //    movie.DownloadCount++;
        //    db.Update(movie);
        //    db.SaveChanges();
        //    var path = "~/images/movies/" + movie.Name;
        //    Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
        //    Response.Headers.Add("Cache-Control", "no-cache");
        //    return File(path,movie.ImageUrl, movie.Name);
        //}

        [HttpPost]
        public IActionResult Search(string name)
        {
            return View(Imovie.Search(name));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using WoodyMovie.Data;
using WoodyMovie.Mail;
using WoodyMovie.Models;

namespace WoodyMovie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        private readonly IMailHelper mailHelper;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IMailHelper mailHelper)
        {
            _logger = logger;
            this.db = db;
            this.mailHelper = mailHelper;
        }

        public IActionResult Index(int RequiredPage = 1)
        {
            RequiredPage = RequiredPage < 1 ? 1: RequiredPage;
            const int pageSize=24;
            decimal moviesCount = db.Movies.Count();
            var pagesCount = Math.Ceiling(moviesCount / pageSize);
            if (RequiredPage > pagesCount)
                RequiredPage = 1;
            ViewBag.CurrentPage = RequiredPage;
            ViewBag.PagesCount = pagesCount;
            return View(db.Movies.Skip((RequiredPage - 1) * pageSize).Take(pageSize));
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        private string UserId
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Contact(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                 db.Contact.AddAsync(new Contact
                 {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    Subject = model.Subject,
                    UserID = UserId
                });
                await db.SaveChangesAsync();
                TempData["Message"] = "Message has been sent successfully";


                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine("<h1>Woody Movie</h1>");
                //sb.AppendFormat("Email:{0}", model.Email);
                //sb.AppendLine();
                //sb.AppendFormat("Subject:{0}",model.Subject);
                //sb.AppendFormat("Message:{0}", model.Message);
                //mailHelper.SendMail(new InputEmailMessage
                //{
                //    Subject = "Thank you for contact us",
                //    Email = "info@woody.com",
                //    Body=sb.ToString()
                //});
                return RedirectToAction("Contact");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SetLang(string lang,string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
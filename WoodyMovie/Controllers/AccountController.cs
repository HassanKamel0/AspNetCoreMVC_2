using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WoodyMovie.Data;
using WoodyMovie.Models;

namespace WoodyMovie.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login model, string? url = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true,true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(url))
                        return LocalRedirect(url);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return View(model);
            }
            else
                return View(model);

        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register model, string? url = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    if (!string.IsNullOrEmpty(url))
                        return LocalRedirect(url);
                    else
                        return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            else
                return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ExternalLogin(string provider) //provider = Facebook or Google
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, "/Account/ExternalResponse");
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalResponse()
        {
            var info =await signInManager.GetExternalLoginInfoAsync();
            if (info==null)
            {
                TempData["Message"] = "Login Failed";
                return RedirectToAction("Login");
            }
            var loginResult =await signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey,false);
            if (!loginResult.Succeeded)
            {
                var userToCreate = new ApplicationUser
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                };
                var createResult = await userManager.CreateAsync(userToCreate);
                if(createResult.Succeeded)
                {
                    var externalLoginResult=await userManager.AddLoginAsync(userToCreate, info);
                    if (externalLoginResult.Succeeded)
                    {
                        await signInManager.SignInAsync(userToCreate, false, info.LoginProvider);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        await userManager.DeleteAsync(userToCreate);
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Info()
        {
            var currentUser = userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var model = mapper.Map<UserViewModel>(currentUser);
                return View(model);
            }
            return NotFound();
        }
        public IActionResult IsEmailExists(string email)
        {
            return Json(true);
        }
    }
}
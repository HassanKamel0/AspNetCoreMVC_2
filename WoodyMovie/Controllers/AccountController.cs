using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Text;
using WoodyMovie.Data;
using WoodyMovie.Mail;
using WoodyMovie.Models;
using WoodyMovie.Resources;

namespace WoodyMovie.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IMailHelper mailHelper;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper, IStringLocalizer<SharedResource> localizer
            ,IMailHelper mailHelper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.localizer = localizer;
            this.mailHelper = mailHelper;
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
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(url))
                        return LocalRedirect(url);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else if (result.IsNotAllowed)
                    TempData["Error"] = localizer["RequireEmailConfirmation"]?.Value;
                else
                    return View(model);
            }
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
                    var token=await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var ConfirmUrl = Url.Action("ConfirmEmail", "Account", new { token= token, userId=user.Id },Request.Scheme) ;
                    StringBuilder body=new StringBuilder();
                    body.AppendLine("Woody Movie website: Email Confirmation");
                    body.AppendFormat("to confirm your email, you should <a href='{0}'> Click here </a>",ConfirmUrl);
                    mailHelper.SendMail(new InputEmailMessage
                    {
                        Body = body.ToString(),
                        Email = model.Email,
                        Subject ="Email Confirmation"
                    });

                    //await signInManager.SignInAsync(user, false);
                    //if (!string.IsNullOrEmpty(url))
                    //    return LocalRedirect(url);
                    //else
                    return RedirectToAction("RequireEmailConfirm");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult RequireEmailConfirm()
        {
            return View();
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
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Message"] = "Login Failed";
                return RedirectToAction("Login");
            }
            var loginResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (!loginResult.Succeeded)
            {
                var userToCreate = new ApplicationUser
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(userToCreate);
                if (createResult.Succeeded)
                {
                    var externalLoginResult = await userManager.AddLoginAsync(userToCreate, info);
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
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var model = mapper.Map<UserViewModel>(currentUser);
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Info(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    currentUser.FirstName = model.FirstName;
                    currentUser.LastName = model.LastName;
                    var result = await userManager.UpdateAsync(currentUser);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = localizer["SuccessMessage"]?.Value;
                        return RedirectToAction("Info");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                    return NotFound();
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (ModelState.IsValid)
                {
                    var result = await userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = localizer["ChangePasswordMessage"]?.Value;
                        await signInManager.SignOutAsync();
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
                return NotFound();
            return View("Info", mapper.Map<UserViewModel>(currentUser));
        }
        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordVM model)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (ModelState.IsValid)
                {
                    var result = await userManager.AddPasswordAsync(currentUser, model.Password);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = localizer["AddPasswordMessage"]?.Value;
                        return RedirectToAction("Info");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
                return NotFound();
            return View("Info", mapper.Map<UserViewModel>(currentUser));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM model)
        {
            if (ModelState.IsValid)
            {
                var user =await userManager.FindByIdAsync(model.UserID);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        
                        var result =await userManager.ConfirmEmailAsync(user, model.Token);
                        if (result.Succeeded)
                        {
                            TempData["Success"] = localizer["Confirm Done"]?.Value;
                            return RedirectToAction("Login");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    else
                        TempData["Success"] = localizer["Confirmed"]?.Value;
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var existedUser = await userManager.FindByEmailAsync(model.Email);
                if(existedUser!=null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(existedUser);
                    var url = Url.Action("ResetPassword", "Account", new { token, model.Email }, Request.Scheme);

                    StringBuilder body = new StringBuilder();
                    body.AppendLine("Woody Movie Application: Reset Password");
                    body.AppendLine("We are sending this email, because we have received a reset password request to your account");
                    body.AppendFormat("To reset new password <a href='{0}'>Click this link</a>",url);
                    mailHelper.SendMail(new InputEmailMessage
                    {
                        Email = model.Email,
                        Subject = "Reset Password",
                        Body = body.ToString()
                    });
                }
                TempData["Success"] = localizer["ReceiveEmail"]?.Value;
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(VerifyResetPasswordVM model)
        {
            if (ModelState.IsValid)
            { 
                var existedUser=await userManager.FindByEmailAsync(model.Email);
                if (existedUser != null)
                { 
                    var isValid=await userManager.VerifyUserTokenAsync(existedUser,TokenOptions.DefaultProvider, "ResetPassword", model.Token);
                    if (isValid)
                    {
                        return View(new ResetPasswordVM {Email=model.Email,Token=model.Token});
                    }
                    else
                        TempData["Error"] = localizer["TokenInvalid"]?.Value;
                }
            }
                return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            //if (ModelState.IsValid)
            //{
                var existedUser = await userManager.FindByEmailAsync(model.Email);
                if (existedUser != null)
                {
                    var result = await userManager.ResetPasswordAsync(existedUser, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = localizer["ResetSuccessfully"]?.Value;
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            //}
            return View(model);
        }
        public IActionResult IsEmailExists(string email)
        {
            return Json(true);
        }
    }
}

using Blog.Data.Entities;
using Blog.Models;
using Blog.Models.ViewModels.AccountViewModels;
using Blog.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User
            {
                Email = model.Email, // в имейле
                UserName = model.Email // и имени будет ...имейл
            };

            // добавляем пользователя
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            // генерация токена для пользователя
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string callbackUrl = Url.Action(
                "ConfirmEmail", // сюда нас перенаправит по ссылке при 
                "Account",      // подтверждении почты в Gmail
                new
                {
                    userId = user.Id, // так же мы передадим id юзера
                    code = code // и токен
                },
                protocol: HttpContext.Request.Scheme)!;

            try
            {
                // отправка gmail
                await _emailService.SendEmailAsync(
                    nameof(Blog),
                    model.Email,
                    "Confirm your account",
                    $"For confirm registration " +
                    $"<a href='{callbackUrl}'>follow the link</a>");

                return View("ConfirmRegistration");
            }
            catch (Exception ex)
            {
                var userToDelete = _userManager.FindByIdAsync(user.Id);
                if (userToDelete is not null)
                {
                    await _userManager.DeleteAsync(user);
                }

                return View("Error", new ErrorViewModel
                {
                    RequestId = ex.Message,
                });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = "An error with confirm your email...",
                });
            }

            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = $"user with id={userId} could not be found...",
                });
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                //return RedirectToAction("Index", "Home");
                return View("ConfirmedAccount");
            }
            else
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = "An error with confirm your email...",
                });
            }
        }



        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await _userManager.FindByNameAsync(model.Email);
            // или же FindByEmailAsync(model.Email)

            if (user != null)
            {
                // проверяем, подтвержден ли email
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "You are not confirmed your Email");

                    return View(model);
                }
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(
                    userName: model.Email,
                    password: model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

            // isPersistent - Флаг, указывающий, должен ли файл cookie для входа
            //  сохраняться после закрытия браузера.
            // lockoutOnFailure - Флаг, указывающий, следует ли блокировать
            //  учетную запись пользователя в случае сбоя входа.


            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid login and (or) password");

                return View(model);
            }
            //избегагаем перенаправления на нежелательные сайты
            if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                 Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManagerController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MainViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.LoginView.Password, model.LoginView.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.LoginView.ReturnUrl) && Url.IsLocalUrl(model.LoginView.ReturnUrl))
                    {
                        return Redirect(model.LoginView.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            StringBuilder values = new();
            foreach (var item in ModelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    values.AppendLine($"{error.ErrorMessage} _______ {error.Exception}");
                }
            }
            Console.WriteLine(values);

            return View("Views/Home/Index.cshtml");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

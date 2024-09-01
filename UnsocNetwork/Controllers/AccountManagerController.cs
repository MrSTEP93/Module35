using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            /*
            // Удаляем ошибки, связанные с RegisterViewModel
            ModelState.Remove("RegisterView.FirstName");
            ModelState.Remove("RegisterView.LastName");
            ModelState.Remove("RegisterView.EmailReg");
            ModelState.Remove("RegisterView.PasswordReg");
            ModelState.Remove("RegisterView.PasswordConfirm");
            ModelState.Remove("RegisterView.Date");
            ModelState.Remove("RegisterView.Month");
            ModelState.Remove("RegisterView.Year");
            */
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        //TempData["MainViewModel"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View("Login", model);
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
            //TempData["MainViewModel"] = JsonConvert.SerializeObject(model);
            Console.WriteLine(values);
            return View("Login", model);
            //return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Route("MyProfile")]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = User;
            //var model = new UserViewModel(user);
            return View("User");
        }
    }
}

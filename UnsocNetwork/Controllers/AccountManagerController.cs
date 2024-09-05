using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Extensions;
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
                        return RedirectToAction("MyProfile", "AccountManager");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View("Login", model);
                }
            }
            return View("Login", model);
            //return RedirectToAction("Index", "Home");
            /*
            StringBuilder values = new();
            foreach (var item in ModelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    values.AppendLine($"{error.ErrorMessage} _______ {error.Exception}");
                }
            }
            //TempData["MainViewModel"] = JsonConvert.SerializeObject(model);
            Console.WriteLine(values);*/
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
            var user = await _userManager.GetUserAsync(User);
            var model = new UserViewModel(user);
            return View("User", model);
        }

        [Authorize]
        [Route("EditProfile")]
        public async Task<IActionResult> ShowEditUserForm()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = _mapper.Map<User, UserEditViewModel>(user);
            return View("UserEdit", model);
        }

        [Authorize]
        [Route("EditProfile")]
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = _mapper.Map<User>(model);

                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                model.IsAttempted = true;
                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    model.Result = "Information updated";
                } else
                {
                    model.Result = "Some errors occured!";
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            } else 
            { 
                ModelState.AddModelError("", "Некорректные данные");
            }
            return View("UserEdit", model);
        }
    }
}

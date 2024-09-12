using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Extensions;
using UnsocNetwork.Models;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IUnitOfWork _unitOfWork;

        public AccountManagerController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
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
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        //&& Url.IsLocalUrl(model.ReturnUrl))
                        //return Redirect(model.ReturnUrl);
                        var uri = new Uri(model.ReturnUrl);
                        return Redirect(uri.AbsoluteUri);
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
        public async Task<IActionResult> MyProfile(string notifySuccess = "", string notifyDanger = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            var model = new UserViewModel(currentUser) 
            { 
                Friends = repository.GetFriendsByUser(currentUser),
                NotifySuccess = notifySuccess, 
                NotifyDanger = notifyDanger
            };
            return View("User", model);
        }

        [Route("EditProfile")]
        [HttpGet]
        public async Task<IActionResult> ShowEditUserForm()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                var model = _mapper.Map<User, UserEditViewModel>(user);
                return View("UserEdit", model);
            }
            else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager",
                        new { returnUrl = $"{Request.Scheme}://{Request.Host}/EditProfile" });
            }
            // https://localhost:5001/EditProfile
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
                    model.NotifySuccess = "Information updated";
                } else
                {
                    model.NotifyDanger = "Some errors occured!";
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

        [Route("Unauthorized")]
        [HttpGet]
        public async Task<IActionResult> ShowAuthForm(string returnUrl)
        {
            var model = new LoginViewModel() { ReturnUrl = returnUrl };
            // string message, string ReturnUrl
            return View("Unauthorized", model);
        }
    }
}

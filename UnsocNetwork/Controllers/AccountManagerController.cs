using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Extensions;
using UnsocNetwork.Models;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.ViewModels.Account;
using UnsocNetwork.ViewModels.UserVM;

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
                IsCurrentUser = true,
                NotifySuccess = notifySuccess, 
                NotifyDanger = notifyDanger
            };
            return View("/Views/User/User.cshtml", model);
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
        }

        [Authorize]
        [Route("EditProfile")]
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
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
        public IActionResult ShowAuthForm(string returnUrl)
        {
            var model = new LoginViewModel() { ReturnUrl = returnUrl };
            return View("Unauthorized", model);
        }

        /// <summary>
        /// Internal service method for manual password change
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            /*
            var user = await _userManager.FindByIdAsync("6a174e9d-8aab-49aa-9045-3cc878305d76");
            var newPass = "111";
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPass);
            await Console.Out.WriteLineAsync(result.Succeeded.ToString());
            //return View("/Views/Home/Index.cshtml");
            */
            return RedirectToAction("Index", "Home");
        }

        /*
        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var usergen = new GenetateUsers();
            var userlist = usergen.Populate(35);

            foreach (var user in userlist)
            {
                var result = await _userManager.CreateAsync(user, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }
        */
    }
}

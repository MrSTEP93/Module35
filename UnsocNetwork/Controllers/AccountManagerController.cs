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
        public async Task<IActionResult> MyProfile(string notifySuccess = "", string notifyDanger = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            //var friends 
            var model = new UserViewModel(currentUser) 
            { 
                Friends = GetAllFriends(currentUser).Result,
                NotifySuccess = notifySuccess, 
                NotifyDanger = notifyDanger
            };
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

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(
            [Required(ErrorMessage = "Введите поисковый запрос")]
            [Display(Name = "Найти...")]
            [StringLength(100, ErrorMessage = "Запрос должен содержать от {2} до {1} символов.", MinimumLength = 1)]
            string searchString)
        {
            var model = new SearchViewModel()
            {
                UserList = new List<UserWithFriendExt>(),
                SearchString = searchString
            };
            if (ModelState.IsValid)
            {
                model = await CreateSearch(searchString);
            }
            return View("UserList", model);
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var searchList = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            var allFriends = await GetAllFriends(currentUser);

            var userListWithFriends = new List<UserWithFriendExt>();
            
            searchList.ForEach(person =>
            {
                var entry = _mapper.Map<UserWithFriendExt>(person);
                entry.IsCurrentUser = (entry.Id == currentUser.Id) ? true : false;
                if (!entry.IsCurrentUser)
                    entry.IsFriendWithCurrent = allFriends.Where(y => y.Id == person.Id || person.Id == currentUser.Id).Count() != 0;
                userListWithFriends.Add(entry);
            });
            
            /*
            foreach (var person in searchList)
            {
                var entry = _mapper.Map<UserWithFriendExt>(person);
                var count = 0;
                foreach (var friend in allFriends) 
                {
                    if (friend.Id == person.Id || person.Id == currentUser.Id)
                    {
                        count++;
                    }
                }

                entry.IsFriendWithCurrent = count != 0;
                userListWithFriends.Add(entry);
            }
            */
            var model = new SearchViewModel()
            {
                UserList = userListWithFriends,
                SearchString = search
            };

            return model;
        }

        private async Task<List<User>> GetAllFriends()
        {
            var user = await _userManager.GetUserAsync(User);

            return GetAllFriends(user).Result;
        }

        private async Task<List<User>> GetAllFriends(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var newFriend = await _userManager.FindByIdAsync(id);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            repository.AddFriend(currentUser, newFriend);

            //return View("UserList", model);
            return RedirectToAction("MyProfile", "AccountManager", 
                new { notifySuccess = $"Friend {newFriend.FirstName} {newFriend.LastName} successfully added" });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var newFriend = await _userManager.FindByIdAsync(id);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            repository.DeleteFriend(currentUser, newFriend);

            //return View("UserList", model);
            return RedirectToAction("MyProfile", "AccountManager", 
                new { notifySuccess = $"Friend {newFriend.FirstName} {newFriend.LastName} successfully deleted" });
        }


    }
}

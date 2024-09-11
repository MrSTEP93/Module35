using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search(
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
            /*
            if (_signInManager.IsSignedIn(User))
            {
            } else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager",
                        new { returnUrl = $"{Request.Scheme}://{Request.Host}/search?SearchString={searchString}" });
            }
            */
        }

        private async Task<SearchViewModel> CreateSearch(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User) ?? new User();

            var searchList = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(searchString.ToLower())).ToList();
            var allFriends = await GetAllFriends(currentUser);

            var userListWithFriends = new List<UserWithFriendExt>();

            searchList.ForEach(person =>
            {
                var entry = _mapper.Map<UserWithFriendExt>(person);
                entry.IsCurrentUser = (entry.Id == currentUser.Id);
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
                SearchString = searchString
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

        [HttpPost]
        public async Task<IActionResult> AddFriend(string id, string searchString)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var newFriend = await _userManager.FindByIdAsync(id);
                var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
                repository.AddFriend(currentUser, newFriend);

                return RedirectToAction("MyProfile", "AccountManager",
                    new { notifySuccess = $"Friend {newFriend.FirstName} {newFriend.LastName} successfully added" });
            } else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager",
                        new { returnUrl = $"{Request.Scheme}://{Request.Host}/search?searchString={searchString}" });
            }
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


        [Authorize]
        [HttpGet]
        public IActionResult FillFriends()
        {
            var userList = _userManager.Users.ToList();
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            foreach (var user in userList)
            {
                repository.AddFriend(user, user);
                /*
                var friendEntity = new Friend()
                {
                    CurrentFriend = user,
                    CurrentFriendId = user.Id,
                    User = user,
                    UserId = user.Id
                };
                */
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

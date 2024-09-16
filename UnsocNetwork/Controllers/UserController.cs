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
        }

        private async Task<SearchViewModel> CreateSearch(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User) ?? new User();
            var allFriends = GetAllFriends(currentUser);
            var userListWithFriends = new List<UserWithFriendExt>();
            var searchList = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(searchString.ToLower())).ToList();

            searchList.ForEach(person =>
            {
                var entry = _mapper.Map<UserWithFriendExt>(person);
                entry.IsCurrentUser = (entry.Id == currentUser.Id);
                if (!entry.IsCurrentUser)
                    entry.IsFriendWithCurrent = allFriends.Where(y => y.Id == person.Id || person.Id == currentUser.Id).Count() != 0;
                userListWithFriends.Add(entry);
            });

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
            return GetAllFriends(user);
        }

        private List<User> GetAllFriends(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            return repository.GetFriendsByUser(user);
        }

        [Route("Person")]
        [HttpGet]
        public async Task<IActionResult> ShowPerson(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (id == currentUser.Id)
            {
                return RedirectToAction("MyProfile", "AccountManager");
            }

            var person = await _userManager.FindByIdAsync(id);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            var model = new UserViewModel(person)
            {
                Friends = repository.GetFriendsByUser(person),
                IsCurrentUser = false,
                NotifySuccess = "",
                NotifyDanger = ""
            };
            return View("User", model);
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

        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var newFriend = await _userManager.FindByIdAsync(id);
                var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
                repository.DeleteFriend(currentUser, newFriend);

                return RedirectToAction("MyProfile", "AccountManager",
                    new { notifySuccess = $"Friend {newFriend.FirstName} {newFriend.LastName} successfully deleted" });
            } else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager");
            }
        }

        /// <summary>
        /// Internal service method (probably, out-of-date)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult FillFriends()
        {
            /*
            var userList = _userManager.Users.ToList();
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            foreach (var user in userList)
            {
                repository.AddFriend(user, user);
            }
            */
            return RedirectToAction("Index", "Home");
        }
    }
}

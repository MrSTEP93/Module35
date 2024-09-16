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
using UnsocNetwork.ViewModels.UserVM;
using System.Net.Http.Headers;

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

        [Authorize]
        [Route("MyProfile")]
        [HttpGet]
        public async Task<IActionResult> MyProfile(string notifySuccess = "", string notifyDanger = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            var model = new UserViewModel(currentUser)
            {
                FriendsList = repository.GetFriendsByUser(currentUser),
                IsCurrentUser = true,
                NotifySuccess = notifySuccess,
                NotifyDanger = notifyDanger
            };
            return View("User", model);
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
                PersonList = new PersonListWithFriendFlags(),
                SearchString = searchString
            };
            if (ModelState.IsValid)
            {
                model = await CreateSearch(searchString);
            }
            return View("Search", model);
        }

        private async Task<SearchViewModel> CreateSearch(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User) ?? new User();
            
            var userListWithFriends = new List<UserWithFriendExt>();
            var searchList = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(searchString.ToLower())).ToList();
            /*
             * var allFriends = GetAllFriends(currentUser);
            searchList.ForEach(person =>
            {
                var entry = _mapper.Map<UserWithFriendExt>(person);
                entry.IsCurrentUser = (entry.Id == currentUser.Id);
                if (!entry.IsCurrentUser)
                    entry.IsFriendWithCurrent = allFriends.Where(y => y.Id == person.Id || person.Id == currentUser.Id).Count() != 0;
                userListWithFriends.Add(entry);
            });
            */
            var model = new SearchViewModel()
            {
                PersonList = FillPersonList(searchList, currentUser),
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

        private PersonListWithFriendFlags FillPersonList(List<User> list, User currentUser)
        {
            var allFriends = GetAllFriends(currentUser);
            PersonListWithFriendFlags result = new();
            //var data = new List<UserWithFriendExt>();
            list.ForEach(person =>
            {
                var element = _mapper.Map<UserWithFriendExt>(person);
                element.IsFriendWithCurrent = allFriends.Where(y => y.Id == person.Id || person.Id == currentUser.Id).Count() != 0;
                result.List.Add(element);
            });

            return result;
        }

        [Route("Person")]
        [HttpGet]
        public async Task<IActionResult> ShowPerson(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (id == currentUser.Id)
            {
                return RedirectToAction("MyProfile");
            }

            var person = await _userManager.FindByIdAsync(id);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            var model = new UserViewModel(person)
            {
                FriendsList = repository.GetFriendsByUser(person),
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

                return RedirectToAction("MyProfile",
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

                return RedirectToAction("MyProfile",
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

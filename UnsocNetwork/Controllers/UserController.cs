using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.Models;
using System.Linq;

namespace UnsocNetwork.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            //_mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
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

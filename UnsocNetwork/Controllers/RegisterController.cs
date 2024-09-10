using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Models;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class RegisterController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Home/Register");
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);
                user.RegDate = DateTime.Now;

                var result = await _userManager.CreateAsync(user, model.PasswordReg);
                if (result.Succeeded)
                {
                    var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
                    repository.AddFriend(user, user);

                    await _signInManager.SignInAsync(user, false);
                    var mainViewModel = new MainViewModel() { 
                        LoginView = new() { Email = model.Email}, 
                        RegisterView = new(), 
                        RegStatusView = new(model.FirstName, true) };
                    TempData["MainViewModel"] = JsonConvert.SerializeObject(mainViewModel);

                    return RedirectToAction("MyProfile", "AccountManager");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            StringBuilder values = new();
            foreach (var item in ModelState.Values)
            {
                foreach(var error in item.Errors)
                {
                    values.AppendLine($"{error.ErrorMessage} _______ {error.Exception}");
                }
            }
            Console.WriteLine(values.ToString());
            return View("RegisterPart2", model);
        }

        [Route("RegisterPart2")]
        [HttpGet]
        public IActionResult RegisterPart2(RegisterViewModel model)
        {
            return View("RegisterPart2", model);
        }
    }
}

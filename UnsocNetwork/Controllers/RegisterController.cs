using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System;
using System.Text;
using System.Threading.Tasks;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class RegisterController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public RegisterController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
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
            /*
            ModelState.ClearValidationState(nameof(LoginViewModel));
            if (TryValidateModel( (model.RegisterView, nameof(MainViewModel.RegisterView)))
            ModelState.ClearValidationState(nameof(LoginViewModel));
            if (TryValidateModel(model.RegisterView, nameof(model.RegisterView)))
            */
            
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var result = await _userManager.CreateAsync(user, model.PasswordReg);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var mainViewModel = new MainViewModel() { 
                        LoginView = new(), 
                        RegisterView = model, 
                        RegStatusView = new(model.FirstName, true) };
                    return RedirectToAction("Index", "Home", mainViewModel);
                    //return View("Home/Index.cshtml", model);
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
        public IActionResult RegisterPart2(MainViewModel model)
        {
            return View("RegisterPart2", model.RegisterView);
        }
    }
}

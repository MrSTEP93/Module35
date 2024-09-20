using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UnsocNetwork.Models;
using System.Threading.Tasks;
using UnsocNetwork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using UnsocNetwork.Data.Repositories;
using UnsocNetwork.Data.UoW;

namespace UnsocNetwork.Controllers
{
    public class MessageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public MessageController(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Route("Chat")]
        public async Task<IActionResult> Chat(string id)
        {
            if (_signInManager.IsSignedIn(User))
            { 
                var currentUser = await _userManager.GetUserAsync(User);
                var recipient = await _userManager.FindByIdAsync(id);
            
                var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;
                var model = new ChatViewModel()
                {
                    Messages = repo.GetMessages(currentUser.Id, id),
                    Recipient = recipient
                };

                return View("Chat", model);
            }
            else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager",
                        new { returnUrl = $"{Request.Scheme}://{Request.Host}/MyProfile" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage(string recipientId,
                    [StringLength(255, ErrorMessage = "Сообщение должно содержать от {2} до {1} символов.", MinimumLength = 1)] 
                    string text)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;
                repo.SendMessage(currentUser.Id, recipientId, text);

                return RedirectToAction("Chat", "Message", new {id = recipientId });
            }
            else
            {
                return RedirectToAction("ShowAuthForm", "AccountManager",
                        new { returnUrl = $"{Request.Scheme}://{Request.Host}/MyProfile" });
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UnsocNetwork.Models.Repositories;
using UnsocNetwork.Models;
using System.Threading.Tasks;
using UnsocNetwork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace UnsocNetwork.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public MessageController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [Route("Chat")]
        public async Task<IActionResult> Chat(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;
            var model = new ChatViewModel()
            {
                Messages = repo.GetMessages(currentUser.Id, id),
                RecepientId = id
            };

            return View("Chat", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage(string recepientId,
                    [StringLength(255, ErrorMessage = "Сообщение должно содержать от {2} до {1} символов.", MinimumLength = 1)] 
                    string text)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;
            repo.SendMessage(currentUser.Id, recepientId, text);

            return RedirectToAction("Chat", "Message", new {id = recepientId });
        }
    }
}

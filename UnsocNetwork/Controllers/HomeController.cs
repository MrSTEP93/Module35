using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /*
        public IActionResult Index(MainViewModel model)
        {
            if (model == null)
            {
                model = new MainViewModel();
            }
            return View(model);
        }
        */

        public IActionResult Index()
        {
            string json = TempData["MainViewModel"] as string;

            if (json != null)
            {
                var model = JsonConvert.DeserializeObject<MainViewModel>(json);
                return View(model);
            }

            return View(new MainViewModel());
        }

        [HttpGet]
        [Route("SuccessRegistration")]
        public IActionResult Welcome()
        {
            return View();
        }

        [HttpGet]
        [Route("index")]
        public IActionResult Develop()
        {
            string status = "Here you are!!!";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

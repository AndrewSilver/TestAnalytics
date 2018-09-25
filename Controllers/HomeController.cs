using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace smartAnalytics.Controllers
{
    [Route("/")]
    public class HomeController:Controller
    {
        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
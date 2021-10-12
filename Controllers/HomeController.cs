using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Epicentre.Library;
using Epicentre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly EpicentreDataContext _context;

        public HomeController(EpicentreDataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!UserActions.UserExists(_context))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
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

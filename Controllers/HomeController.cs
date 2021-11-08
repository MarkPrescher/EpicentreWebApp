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
        private readonly UserManager<EpicentreUser> userManager;
        private readonly SignInManager<EpicentreUser> signInManager;

        public HomeController(UserManager<EpicentreUser> userManager, SignInManager<EpicentreUser> signInManager, EpicentreDataContext context)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            // This needs to be done throughout
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            else if (User.IsInRole("Nurse"))
            {
                return RedirectToAction("SearchForPatient", "CovidTests");
            }
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

        public IActionResult ContactHelper(string topic, string message)
        {
            // Send email here

            // If successful, then redirect to successful contact
            // Otherwise redirect to failed contact

            return View();
        }

        public IActionResult SuccessfulContact()
        {
            return View();
        }

        public IActionResult FailedContact()
        {
            return View()
;        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

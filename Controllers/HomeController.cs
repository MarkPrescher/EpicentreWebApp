using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Epicentre.Library;
using Epicentre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Epicentre.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly EpicentreDataContext _context;
        private readonly SignInManager<EpicentreUser> signInManager;

        public HomeController(SignInManager<EpicentreUser> signInManager, EpicentreDataContext context)
        {
            _context = context;
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

        [Authorize(Roles = "User")]
        public IActionResult About()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult ContactHelper(string topic, string message)
        {
            // Send email here

            // If successful, then redirect to successful contact
            // Otherwise redirect to failed contact

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult SuccessfulContact()
        {
            return View();
        }

        [Authorize(Roles = "User")]
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
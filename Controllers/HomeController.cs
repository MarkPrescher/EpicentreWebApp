using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Epicentre.Library;
using Epicentre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
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
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            else if (User.IsInRole("Nurse"))
            {
                return RedirectToAction("SearchForPatient", "CovidTests");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "CovidTests");
            }

            var firstName = _context.UserDetail.Where(u => u.EMAIL_ADDRESS == UserActions.UserEmail).Select(u => u.FIRST_NAME).FirstOrDefault();
            ViewBag.FirstName = firstName;

            var numberOfCovidTests = _context.CovidTest.Where(c => c.USER_EMAIL == UserActions.UserEmail).Count(c => c.USER_EMAIL == UserActions.UserEmail);
            ViewBag.NumberOfTests = numberOfCovidTests;

            var numberOfPositiveCovidTests = _context.CovidTest.Where(c => c.USER_EMAIL == UserActions.UserEmail).Count(c => c.TEST_RESULT == "Positive");
            ViewBag.NumberOfPositiveTests = numberOfPositiveCovidTests;

            var numberOfNegativeCovidTests = _context.CovidTest.Where(c => c.USER_EMAIL == UserActions.UserEmail).Count(c => c.TEST_RESULT == "Negative");
            ViewBag.NumberOfNegativeTests = numberOfNegativeCovidTests;

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult About()
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult Contact()
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult ContactHelper(string topic, string message)
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            try
            {
                string Data;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("noreplyepicentertest@gmail.com", "Epicentre");
                mail.To.Add("noreplyepicentertest@gmail.com");
                mail.Subject = topic;
                Data = "User: " + UserActions.UserEmail + "\n" + "Message: " + message;

                mail.Body = Data;
                smtpClient.Send(mail);
                return RedirectToAction(nameof(SuccessfulContact));
            }
            catch (Exception exception)
            {
                return RedirectToAction(nameof(FailedContact));
            }
            // If successful, then redirect to successful contact
            // Otherwise redirect to failed contact

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

        [Authorize(Roles = "Admin")]
        public IActionResult AddNurse()
        {
            return View();
        }

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
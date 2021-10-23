using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Epicentre.Models;
using Epicentre.Library;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Epicentre.Controllers;

namespace Epicentre.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly EpicentreDataContext _context;
        private readonly UserManager<EpicentreUser> _userManager;
        private readonly SignInManager<EpicentreUser> _signInManager;

        public IndexModel(
            UserManager<EpicentreUser> userManager,
            SignInManager<EpicentreUser> signInManager,
            EpicentreDataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(EpicentreUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (UserActions.UserEmail == "")
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                Response.Redirect(url);
            }

            var details = _context.UserDetail.FirstOrDefault(m => m.EMAIL_ADDRESS == UserActions.UserEmail);

            ViewData["FirstName"] = details.FIRST_NAME;
            ViewData["LastName"] = details.LAST_NAME;
            ViewData["IDNumber"] = details.ID_NUMBER;
            ViewData["ContactNumber"] = details.CONTACT_NUMBER;
            ViewData["EmailAddress"] = details.EMAIL_ADDRESS;
            ViewData["Gender"] = details.GENDER;
            ViewData["MedicalAid"] = details.MEDICAL_AID;
            ViewData["MembershipNumber"] = details.MEMBERSHIP_NUMBER;
            ViewData["AuthorizationNumber"] = details.AUTH_NUMBER;

            ViewData["StatusCode"] = Status.StatusCode.ToString();
            if (Status.StatusCode == 1)
            {
                ViewData["StatusMessage"] = "Your account has been updated";
            }
            Status.StatusCode = 0;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }
    }
}

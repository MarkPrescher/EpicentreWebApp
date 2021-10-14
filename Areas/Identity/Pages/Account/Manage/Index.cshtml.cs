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

            // query here
            //ViewData["FirstName"] = "";

            var details =  _context.UserDetail.FirstOrDefault(m => m.EMAIL_ADDRESS == UserActions.UserEmail);

            ViewData["FirstName"] = details.FIRST_NAME;
            ViewData["Last"] = details.LAST_NAME;
            ViewData["ID"] = details.ID_NUMBER;
            ViewData["Contact"] = details.CONTACT_NUMBER;
            ViewData["Email"] = details.EMAIL_ADDRESS;
            ViewData["Gender"] = details.GENDER;
            ViewData["Medical"] = details.MEDICAL_AID;
            ViewData["Member"] = details.MEMBERSHIP_NUMBER;
            ViewData["Auth"] = details.AUTH_NUMBER;


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

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task <IActionResult> save(string first)
        {
            var details = _context.UserDetail.FirstOrDefault(m => m.EMAIL_ADDRESS == UserActions.UserEmail);


            details.FIRST_NAME = "F";
             _context.UserDetail.Update(details);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Identity/Account/Manage");

        }
    }
}

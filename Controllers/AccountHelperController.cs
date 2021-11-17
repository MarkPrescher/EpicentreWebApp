using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Epicentre.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Controllers
{
    [Authorize]
    public class AccountHelperController : Controller
    {
        private readonly EpicentreDataContext _context;
        private readonly UserManager<EpicentreUser> _userManager;
        public AccountHelperController(EpicentreDataContext context, UserManager<EpicentreUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> UpdateProfile(string firstName, string lastName, string idNumber, string contactNumber, string gender, string medicalAid, string membershipNumber, string authorizationNumber)
        {
            var details = _context.UserDetail.FirstOrDefault(m => m.EMAIL_ADDRESS == UserActions.UserEmail);
            details.FIRST_NAME = firstName;
            details.LAST_NAME = lastName;
            details.ID_NUMBER = idNumber;
            details.CONTACT_NUMBER = contactNumber;
            details.GENDER = gender;
            details.MEDICAL_AID = medicalAid;
            details.MEMBERSHIP_NUMBER = membershipNumber;
            details.AUTH_NUMBER = authorizationNumber;

            _context.UserDetail.Update(details);
            await _context.SaveChangesAsync();
            var url = Url.Page(
                    "/Account/Manage/Index",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
            Status.StatusCode = 1;
            return Redirect(url);
        }

        public async Task<IActionResult> AddNurse(string emailAddress, string password)
        {
            var user = new EpicentreUser { UserName = emailAddress, Email = emailAddress, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, "Nurse");

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return RedirectToAction("AddNurse", "Home");
            }
        }
    }
}
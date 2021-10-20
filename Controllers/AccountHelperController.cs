using Epicentre.Data;
using Epicentre.Library;
using Microsoft.AspNetCore.Authorization;
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

        public AccountHelperController(EpicentreDataContext context)
        {
            _context = context;
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
            return Redirect(url);
        }
    }
}
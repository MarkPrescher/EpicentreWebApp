using Epicentre.Data;
using Epicentre.Library;
using Epicentre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Controllers
{
    [Authorize(Roles = "User")]
    public class UserDetailsController : Controller
    {
        private readonly EpicentreDataContext _context;

        public UserDetailsController(EpicentreDataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // check if already completed, otherwise user should not be able to see these pages
            if (UserActions.UserEmail == null)
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                return Redirect(url);
            }
            UserInformationDetails.EmailAddress = UserActions.UserEmail;

            return View();
        }

        
        public IActionResult ContactDetailsHelper(string firstName, string lastName, string idNumber)
        {
            if (firstName == null || lastName == null || idNumber == null)
            {
                return NotFound();
            }
            if (UserActions.UserEmail == null)
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                return Redirect(url);
            }
            UserInformationDetails.FirstName = firstName.ToString();
            UserInformationDetails.LastName = lastName.ToString();
            UserInformationDetails.IdNumber = idNumber.ToString();
            return RedirectToAction("ContactDetails", "UserDetails");
        }

        public IActionResult ContactDetails()
        {
            if (UserActions.UserEmail == null)
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                return Redirect(url);
            }
            ViewBag.EmailAddress = UserActions.UserEmail;
            ViewBag.FirstName = UserInformationDetails.FirstName;
            return View();
        }

        public IActionResult MedicalAidDetailsHelper(string contactNumber, string gender)
        {
            if (contactNumber == null || gender == null)
            {
                return NotFound();
            }
            if (UserActions.UserEmail == null)
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                return Redirect(url);
            }
            UserInformationDetails.ContactNumber = contactNumber.ToString();
            UserInformationDetails.Gender = gender.ToString();
            return RedirectToAction("MedicalAidDetails", "UserDetails");
        }

        public IActionResult MedicalAidDetails()
        {
            if (UserActions.UserEmail == null)
            {
                var url = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { area = "Identity" },
                    protocol: Request.Scheme);
                return Redirect(url);
            }
            return View();
        }

        public async Task<IActionResult> SaveUserDetails(string medicalAid, string membershipNumber, string authNumber)
        {
            if (medicalAid == null || membershipNumber == null || authNumber == null)
            {
                return NotFound();
            }
            UserInformationDetails.MedicalAid = medicalAid.ToString();
            UserInformationDetails.MembershipNumber = membershipNumber.ToString();
            UserInformationDetails.AuthNumber = authNumber.ToString();

            UserDetail userDetail = new UserDetail();
            userDetail.ID = Guid.NewGuid();
            userDetail.FIRST_NAME = UserInformationDetails.FirstName;
            userDetail.LAST_NAME = UserInformationDetails.LastName;
            userDetail.ID_NUMBER = UserInformationDetails.IdNumber;
            userDetail.CONTACT_NUMBER = UserInformationDetails.ContactNumber;
            userDetail.EMAIL_ADDRESS = UserInformationDetails.EmailAddress;
            userDetail.GENDER = UserInformationDetails.Gender;
            userDetail.MEDICAL_AID = UserInformationDetails.MedicalAid;
            userDetail.MEMBERSHIP_NUMBER = UserInformationDetails.MembershipNumber;
            userDetail.AUTH_NUMBER = UserInformationDetails.AuthNumber;

            try
            {
                await _context.UserDetail.AddAsync(userDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction("UserDetailsSaved", "UserDetails");
            }
            catch (Exception exception)
            {
                return NotFound();
            }
        }

        public IActionResult UserDetailsSaved()
        {
            return View();
        }
    }
}
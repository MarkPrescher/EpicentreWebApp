using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Epicentre.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;

namespace Epicentre.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<EpicentreUser> _userManager;
        static string tokenCode;

        public ResetPasswordModel(UserManager<EpicentreUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your email address")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please enter a password")]
            [StringLength(55, ErrorMessage = "Your password must be at least {2} and at {1} characters long", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset");
            }
            else
            {
                tokenCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, tokenCode, Input.Password);
            if (result.Succeeded)
            {
                MailMessage mailMessage = new MailMessage("noreplyepicentertest@gmail.com", Input.Email.ToString()/*Recipient email*/, "Password Reset", "Your password has successfully been reset.");
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
                await smtpClient.SendMailAsync(mailMessage);
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}

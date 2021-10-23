using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Epicentre.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;
using Epicentre.Library;

namespace Epicentre.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<EpicentreUser> _userManager;
        private readonly SignInManager<EpicentreUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<EpicentreUser> userManager,
            SignInManager<EpicentreUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a new email address")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(EpicentreUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            //Input = new InputModel
            //{
            //    NewEmail = email,
            //};

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            ViewData["StatusCode"] = Status.StatusCode.ToString();
            if (Status.StatusCode == 1)
            {
                ViewData["StatusMessage"] = "Please check your email address to verify your new email address";
            }
            Status.StatusCode = 0;
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

        public async Task<IActionResult> OnPostChangeEmailAsync()
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

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);

                MailMessage mailMessage = new MailMessage("noreplyepicentertest@gmail.com", Input.NewEmail.ToString()/*Recipient email*/, "Confirm Email Change", $"Please confirm your email change:\n {callbackUrl}");
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
                await smtpClient.SendMailAsync(mailMessage);

                Status.StatusCode = 1;

                return RedirectToPage();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            MailMessage mailMessage = new MailMessage("noreplyepicentertest@gmail.com", Input.NewEmail.ToString()/*Recipient email*/, "Confirm Email Change", $"Please confirm your email change:\n {callbackUrl}");
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
            await smtpClient.SendMailAsync(mailMessage);

            Status.StatusCode = 1;

            return RedirectToPage();
        }
    }
}

using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BDFA.Pages
{
    public class LoginModel : PageModel
    {
        public bool ShowSendCode { get; set; } = true;
        public bool ShowSignIn { get; set; } = false;
        public string Email { get; set; }
        public string AuthCode { get; set; }
        //public DateTime Expires { get; set; }

        [BindProperty]
        public string InputEmail { get; set; }

        [BindProperty]
        public string InputAuthCode { get; set; }

        // Setup Profile sqllite table
        public Profile Profile { get; set; }
        // Setup Admin sqllite table
        public Admin Admin { get; set; }

        private readonly ILogger<LoginModel> _logger;
        private readonly DirectoryContext _context;

        public LoginModel(ILogger<LoginModel> logger, DirectoryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;
        }

        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            if (!ModelState.IsValid)
            {
                // Update what is visible on the page
                ShowSignIn = false;
                ShowSendCode = true;

                AuthCode = string.Empty;
                HttpContext.Session.SetInt32("IsAdmin", 0);
                HttpContext.Session.SetInt32("IsAuth", 0);
                HttpContext.Session.SetInt32("IdKey", 0);
                HttpContext.Session.SetString("EmailKey", string.Empty);
                return Page();
            }

            // Update what is visible on the page
            ShowSignIn = true;
            ShowSendCode = false;

            // Generate a new AuthCode
            AuthCode = GenerateOneTimeCode();

            // Check if the email is an admin
            if (await BL.Manager.IsAdminAsync(_context, InputEmail))
            {
                HttpContext.Session.SetInt32("IsAdmin", 1);
            }
            else
            {
                HttpContext.Session.SetInt32("IsAdmin", 0);
            }

            // Create a new Profile object with the given email and AuthCode
            if (HttpContext.Session.GetInt32("IsAdmin") == 0) {
                var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == InputEmail);

                if (existingProfile == null)
                {
                    // If no existing profile is found, add a new one
                    // Instantiate the Profile object
                    Profile = new Profile
                    {
                        // Populate the Profile properties with data
                        Active = true,
                        FeaturedAuthor = false,
                        Email = InputEmail,
                        Password = AuthCode,
                        AuthToken = AuthCode,
                        Expires = DateTime.UtcNow.AddMinutes(15)
                    };
                    // Now, you can add the instantiated Profile to the database context
                    _context.Profiles.Add(Profile);
                }
                else
                {
                    // If an existing profile is found, update its properties
                    existingProfile.AuthToken = AuthCode;
                    existingProfile.Expires = DateTime.UtcNow.AddMinutes(15);
                }

                // Set session variable for the email
                HttpContext.Session.SetString("EmailKey", InputEmail);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == InputEmail);

                if (existingAdmin == null)
                {
                    // Handle the case where no admin is found with the given email
                    // This might involve setting an error message or returning to the page
                    return Page();
                }

                // Set session variable for the email
                HttpContext.Session.SetString("EmailKey", InputEmail);

                // If an existing profile is found, update its properties
                existingAdmin.AuthToken = AuthCode;
                existingAdmin.Expires = DateTime.UtcNow.AddMinutes(15);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            BL.Manager.SendMail(InputEmail, "Login code requested from BDFA", GenerateEmail(AuthCode));

            // Return to page to allow the user to enter the code sent to them via email
            return Page();
        }

        public async Task<IActionResult> OnPostSignInAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (HttpContext.Session.GetInt32("IsAdmin") == 0)
            {
                // Look up the profile by email
                var profile = await _context.Profiles
                                        .FirstOrDefaultAsync(p => p.Email == HttpContext.Session.GetString("EmailKey"));

                if (profile == null)
                {
                    // No profile found with the given email
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                // Check if the AuthCode matches and the Expires timestamp has not passed
                if (profile.AuthToken == InputAuthCode && profile.Expires > DateTime.UtcNow)
                {
                    HttpContext.Session.SetInt32("IsAuth", 1);
                    // Authentication successful
                    // Proceed with sign-in or further actions

                    return RedirectToPage("./Profile"); // Redirect to a success page or another appropriate action
                }
                else
                {
                    // Authentication failed
                    ModelState.AddModelError(string.Empty, "Invalid authentication code or code has expired.");
                    return Page();
                }
            }
            else
            {
                // Look up the admin by email
                var existingAdmin = await _context.Admins
                                        .FirstOrDefaultAsync(p => p.Email == HttpContext.Session.GetString("EmailKey"));

                if (existingAdmin == null)
                {
                    // No profile found with the given email
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                // Check if the AuthCode matches and the Expires timestamp has not passed
                if (existingAdmin.AuthToken == InputAuthCode && existingAdmin.Expires > DateTime.UtcNow)
                {
                    HttpContext.Session.SetInt32("IsAuth", 1);
                    // Authentication successful
                    // Proceed with sign-in or further actions

                    return RedirectToPage("./Admin"); // Redirect to a success page or another appropriate action
                }
                else
                {
                    // Authentication failed
                    ModelState.AddModelError(string.Empty, "Invalid authentication code or code has expired.");
                    return Page();
                }
            }
        }

        private static string GenerateOneTimeCode()
        {
            Random random = new();
            string _authCode = random.Next(100000, 999999).ToString();
            return _authCode;
        }

        private static string GenerateEmail(string _authToken)
        {
            StringBuilder emailBody = new();
            emailBody.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.= w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\" /><meta name=color-scheme content=\"light dark\" /><meta name=supported-color-schemes content=\"light dark\" /><meta name=x-apple-disable-message-reformatting /></head><body style=\"\r\n      margin: 20px;\r\n      padding: 0;\r\n      text-align: center;\r\n      font-family: Helvetica, sans-serif;\r\n    \"><div style=\"height: 240px; margin-bottom: 16px\"><img src=https://buydirectfromauthors.com/img/email-header.png height=240 style=\"height: 240px; width: auto\" /></div><h1 style=\"margin: 0px\">Your login request to BDFA</h1><p style=\"font-size: 18px; margin-bottom: 24px; margin-top: 24px\">###### is your one-time code to log in to your account. Your code expires in 15 minutes.</p></body></html>");
            emailBody.Replace("######", _authToken);

            return emailBody.ToString();
        }
    }
}

using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BDFA.BL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BDFA.Pages
{
    public class LoginModel : PageModel
    {
        #region Properties
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
        #endregion

        public LoginModel(ILogger<LoginModel> logger, DirectoryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            //Logout and reset global variables
            Globals.isAuth = false;
            Globals.isAdmin = false;
            Globals.pId = -1;
            Globals.pEmail = string.Empty;
        }

        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            if (!ModelState.IsValid)
            {
                ShowSignIn = false;
                ShowSendCode = true;
                return Page();
            }
            else
            {
                /////
                // Determine if the email is an admin or a profile
                // Send the auth code to the email
                /////

                ShowSignIn = true;
                ShowSendCode = false;

                Globals.pEmail = InputEmail;
                Globals.isAdmin = await Manager.IsAdmin(_context, InputEmail);
                Globals.pId = await Manager.ProfileExistsByEmail(_context, InputEmail);

                string AuthCode = Manager.GenerateOneTimeCode();

                ////
                // Check to see if the profile exists
                // If it does not exist, create a new profile
                // If it does exist, update the existing profile with auth code and expiration
                // Send the email with the auth code
                // Save the profile to the database
                // Return to the page to allow the user to enter the code sent to them via email
                ////
              
                if (!Globals.isAdmin)
                {
                    // Look up the profile by email
                    var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == InputEmail);

                    if (existingProfile == null)
                    {
                        // If no existing profile is found, create a new one
                        Profile = new Profile
                        {
                            // Populate the Profile properties with initial data
                            Active = true,
                            FeaturedAuthor = false,
                            Email = InputEmail,
                            Password = AuthCode,
                            AuthToken = AuthCode,
                            Expires = DateTime.UtcNow.AddMinutes(15)
                        };
                        _context.Profiles.Add(Profile);
                        // Save changes to the database
                        await _context.SaveChangesAsync();
                        Globals.pId = Profile.Id;
                    }
                    else
                    {
                        // If an existing profile is found, update AuthCode and Expiration
                        existingProfile.AuthToken = AuthCode;
                        existingProfile.Expires = DateTime.UtcNow.AddMinutes(15);
                        // Save changes to the database
                        await _context.SaveChangesAsync();
                        Globals.pId = existingProfile.Id;
                    }
                }
                else
                {
                    // Look up the admin by email
                    var existingAdmin = await _context.Admins.FirstOrDefaultAsync(p => p.Email == InputEmail);

                    // If an existing profile is found, update AuthCode and Expiration
                    existingAdmin.AuthToken = AuthCode;
                    existingAdmin.Expires = DateTime.UtcNow.AddMinutes(15);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }

                // Send the email with the auth code
                Manager.SendMail(InputEmail, "Login code requested from BDFA", GenerateEmail(AuthCode));

                // Return to page to allow the user to enter the code sent to them via email
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSignInAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Login - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ////
            // Check to see if the profile exists
            // If it does not exist, return an error
            // If it does exist, check the auth code and expiration
            // If the auth code is correct and the expiration has not passed, proceed to the profile page
            // If the auth code is incorrect or the expiration has passed, return an error
            ////
            
            if (!Globals.isAdmin)
            {
                // Look up the profile by email
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == Globals.pEmail);

                if (profile == null)
                {
                    // No profile found with the given email
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                else
                {
                    // Check if the AuthCode matches and the expiration has not passed
                    if (profile.AuthToken == InputAuthCode.Trim() && profile.Expires > DateTime.UtcNow)
                    {
                        Globals.isAuth = true;
                        return RedirectToPage("./Profile");
                    }
                    else
                    {
                        // Authentication failed
                        ModelState.AddModelError(string.Empty, "Invalid authentication code or code has expired.");
                        return Page();
                    }
                }
            }
            else
            {
                // Look up the admin by email
                var profile = await _context.Admins.FirstOrDefaultAsync(p => p.Email == Globals.pEmail);

                if(profile == null) {
                    // No profile found with the given email
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                else
                {
                    // Check if the AuthCode matches and the expiration has not passed
                    if (profile.AuthToken == InputAuthCode.Trim() && profile.Expires > DateTime.UtcNow)
                    {
                        Globals.isAuth = true;
                        Globals.isAdmin = true;
                        return RedirectToPage("./Admin");
                    }
                    else
                    {
                        // Authentication failed
                        ModelState.AddModelError(string.Empty, "Invalid authentication code or code has expired.");
                        return Page();
                    }
                }
            }
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

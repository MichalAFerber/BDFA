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
        private readonly BDFA.Data.DirectoryContext _context;

        public LoginModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            if (!ModelState.IsValid)
            {
                // Update what is visible on the page
                ShowSignIn = false;
                ShowSendCode = true;

                AuthCode = string.Empty;
                HttpContext.Session.SetInt32("IsAdmin", 0);
                HttpContext.Session.SetInt32("IsAuth", 0);
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
                        Email = InputEmail,
                        AuthToken = AuthCode,
                        Expires = DateTime.UtcNow.AddMinutes(15)
                        // Set other properties as needed
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
                // Set session variable to the email
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

                HttpContext.Session.SetString("EmailKey", InputEmail);

                // If an existing profile is found, update its properties
                existingAdmin.AuthToken = AuthCode;
                existingAdmin.Expires = DateTime.UtcNow.AddMinutes(15);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            BL.Manager.SendMail(InputEmail, "Login Code from BDFA", GenerateEmail(AuthCode));

            // Return to page to allow the user to enter the code sent to them via email
            return Page();
        }

        public async Task<IActionResult> OnPostSignInAsync()
        {
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
            emailBody.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=https://www.w3.org/1999/xhtml xmlns:v=urn:schemas-microsoft-com:vml xmlns:o=urn:schemas-microsoft-com:office:office><head><meta charset=UTF-8 /><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\" /><meta http-equiv=X-UA-Compatible content=\"IE=edge\" /><meta name=viewport content=\"width=device-width, initial-scale=1.0\" /><meta name=format-detection content=\"telephone=no\" /><meta name=format-detection content=\"date=no\" /><meta name=format-detection content=\"address=no\" /><meta name=format-detection content=\"email=no\" /><meta name=x-apple-disable-message-reformatting /><link href=\"https://fonts.googleapis.com/css?family=Fira+Sans:ital,wght@0,400;0,400;0,800\" rel=stylesheet /><title>Send Login Code</title><style>@media all{@font-face{font-family:'Fira Sans';font-style:normal;font-weight:400;src:local('Fira Sans Regular'),local('FiraSans-Regular'),url(https://fonts.gstatic.com/s/firasans/v10/va9E4kDNxMZdWfMOD5VvmojLazX3dGTP.woff2) format('woff2');unicode-range:U+460-52F,U+1C80-1C88,U+20B4,U+2DE0-2DFF,U+A640-A69F,U+FE2E-FE2F}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:400;src:local('Fira Sans Regular'),local('FiraSans-Regular'),url(https://fonts.gstatic.com/s/firasans/v10/va9E4kDNxMZdWfMOD5Vvk4jLazX3dGTP.woff2) format('woff2');unicode-range:U+400-45F,U+490-491,U+4B0-4B1,U+2116}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:400;src:local('Fira Sans Regular'),local('FiraSans-Regular'),url(https://fonts.gstatic.com/s/firasans/v10/va9E4kDNxMZdWfMOD5VvmYjLazX3dGTP.woff2) format('woff2');unicode-range:U+100-24F,U+259,U+1E00-1EFF,U+2020,U+20A0-20AB,U+20AD-20CF,U+2113,U+2C60-2C7F,U+A720-A7FF}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:400;src:local('Fira Sans Regular'),local('FiraSans-Regular'),url(https://fonts.gstatic.com/s/firasans/v10/va9E4kDNxMZdWfMOD5Vvl4jLazX3dA.woff2) format('woff2');unicode-range:U+0-FF,U+131,U+152-153,U+2BB-2BC,U+2C6,U+2DA,U+2DC,U+2000-206F,U+2074,U+20AC,U+2122,U+2191,U+2193,U+2212,U+2215,U+FEFF,U+FFFD}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:800;font-display:swap;src:local('Fira Sans ExtraBold'),local('FiraSans-ExtraBold'),url(https://fonts.gstatic.com/s/firasans/v10/va9B4kDNxMZdWfMOD5VnMK7eSxf6Xl7Gl3LX.woff2) format('woff2');unicode-range:U+460-52F,U+1C80-1C88,U+20B4,U+2DE0-2DFF,U+A640-A69F,U+FE2E-FE2F}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:800;font-display:swap;src:local('Fira Sans ExtraBold'),local('FiraSans-ExtraBold'),url(https://fonts.gstatic.com/s/firasans/v10/va9B4kDNxMZdWfMOD5VnMK7eQhf6Xl7Gl3LX.woff2) format('woff2');unicode-range:U+400-45F,U+490-491,U+4B0-4B1,U+2116}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:800;font-display:swap;src:local('Fira Sans ExtraBold'),local('FiraSans-ExtraBold'),url(https://fonts.gstatic.com/s/firasans/v10/va9B4kDNxMZdWfMOD5VnMK7eSBf6Xl7Gl3LX.woff2) format('woff2');unicode-range:U+100-24F,U+259,U+1E00-1EFF,U+2020,U+20A0-20AB,U+20AD-20CF,U+2113,U+2C60-2C7F,U+A720-A7FF}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:800;font-display:swap;src:local('Fira Sans ExtraBold'),local('FiraSans-ExtraBold'),url(https://fonts.gstatic.com/s/firasans/v10/va9B4kDNxMZdWfMOD5VnMK7eRhf6Xl7Glw.woff2) format('woff2');unicode-range:U+0-FF,U+131,U+152-153,U+2BB-2BC,U+2C6,U+2DA,U+2DC,U+2000-206F,U+2074,U+20AC,U+2122,U+2191,U+2193,U+2212,U+2215,U+FEFF,U+FFFD}}</style><style>html,body{margin:0!important;padding:0!important;min-height:100%!important;width:100%!important;-webkit-font-smoothing:antialiased}*{-ms-text-size-adjust:100%}#outlook a{padding:0}.ReadMsgBody,.ExternalClass{width:100%}.ExternalClass,.ExternalClass p,.ExternalClass td,.ExternalClass div,.ExternalClass span,.ExternalClass font{line-height:100%}table,td,th{mso-table-lspace:0!important;mso-table-rspace:0!important;border-collapse:collapse}u+.body table,u+.body td,u+.body th{will-change:transform}body,td,th,p,div,li,a,span{-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;mso-line-height-rule:exactly}img{border:0;outline:0;line-height:100%;text-decoration:none;-ms-interpolation-mode:bicubic}a[x-apple-data-detectors]{color:inherit!important;text-decoration:none!important}.pc-gmail-fix{display:none;display:none!important}@media(min-width:621px){.pc-lg-hide{display:none}.pc-lg-bg-img-hide{background-image:none!important}}</style><style>@media(max-width:620px){.pc-project-body{min-width:0!important}.pc-project-container{width:100%!important}.pc-sm-hide{display:none!important}.pc-sm-bg-img-hide{background-image:none!important}.pc-w620-padding-30-30-30-30{padding:30px 30px 30px 30px!important}.pc-w620-fontSize-30{font-size:30px!important}.pc-w620-lineHeight-133pc{line-height:133%!important}.pc-w620-fontSize-18{font-size:18px!important}.pc-w620-lineHeight-156pc{line-height:156%!important}.pc-w620-padding-35-35-35-35{padding:35px 35px 35px 35px!important}.pc-w620-gridCollapsed-1>tbody,.pc-w620-gridCollapsed-1>tbody>tr,.pc-w620-gridCollapsed-1>tr{display:inline-block!important}.pc-w620-gridCollapsed-1.pc-width-fill>tbody,.pc-w620-gridCollapsed-1.pc-width-fill>tbody>tr,.pc-w620-gridCollapsed-1.pc-width-fill>tr{width:100%!important}.pc-w620-gridCollapsed-1.pc-w620-width-fill>tbody,.pc-w620-gridCollapsed-1.pc-w620-width-fill>tbody>tr,.pc-w620-gridCollapsed-1.pc-w620-width-fill>tr{width:100%!important}.pc-w620-gridCollapsed-1>tbody>tr>td,.pc-w620-gridCollapsed-1>tr>td{display:block!important;width:auto!important;padding-left:0!important;padding-right:0!important}.pc-w620-gridCollapsed-1.pc-width-fill>tbody>tr>td,.pc-w620-gridCollapsed-1.pc-width-fill>tr>td{width:100%!important}.pc-w620-gridCollapsed-1.pc-w620-width-fill>tbody>tr>td,.pc-w620-gridCollapsed-1.pc-w620-width-fill>tr>td{width:100%!important}.pc-w620-gridCollapsed-1>tbody>.pc-grid-tr-first>.pc-grid-td-first,pc-w620-gridCollapsed-1>.pc-grid-tr-first>.pc-grid-td-first{padding-top:0!important}.pc-w620-gridCollapsed-1>tbody>.pc-grid-tr-last>.pc-grid-td-last,pc-w620-gridCollapsed-1>.pc-grid-tr-last>.pc-grid-td-last{padding-bottom:0!important}.pc-w620-gridCollapsed-0>tbody>.pc-grid-tr-first>td,.pc-w620-gridCollapsed-0>.pc-grid-tr-first>td{padding-top:0!important}.pc-w620-gridCollapsed-0>tbody>.pc-grid-tr-last>td,.pc-w620-gridCollapsed-0>.pc-grid-tr-last>td{padding-bottom:0!important}.pc-w620-gridCollapsed-0>tbody>tr>.pc-grid-td-first,.pc-w620-gridCollapsed-0>tr>.pc-grid-td-first{padding-left:0!important}.pc-w620-gridCollapsed-0>tbody>tr>.pc-grid-td-last,.pc-w620-gridCollapsed-0>tr>.pc-grid-td-last{padding-right:0!important}.pc-w620-tableCollapsed-1>tbody,.pc-w620-tableCollapsed-1>tbody>tr,.pc-w620-tableCollapsed-1>tr{display:block!important}.pc-w620-tableCollapsed-1.pc-width-fill>tbody,.pc-w620-tableCollapsed-1.pc-width-fill>tbody>tr,.pc-w620-tableCollapsed-1.pc-width-fill>tr{width:100%!important}.pc-w620-tableCollapsed-1.pc-w620-width-fill>tbody,.pc-w620-tableCollapsed-1.pc-w620-width-fill>tbody>tr,.pc-w620-tableCollapsed-1.pc-w620-width-fill>tr{width:100%!important}.pc-w620-tableCollapsed-1>tbody>tr>td,.pc-w620-tableCollapsed-1>tr>td{display:block!important;width:auto!important}.pc-w620-tableCollapsed-1.pc-width-fill>tbody>tr>td,.pc-w620-tableCollapsed-1.pc-width-fill>tr>td{width:100%!important;box-sizing:border-box!important}.pc-w620-tableCollapsed-1.pc-w620-width-fill>tbody>tr>td,.pc-w620-tableCollapsed-1.pc-w620-width-fill>tr>td{width:100%!important;box-sizing:border-box!important}}@media(max-width:520px){.pc-w520-padding-25-25-25-25{padding:25px 25px 25px 25px!important}.pc-w520-padding-30-30-30-30{padding:30px 30px 30px 30px!important}}</style><style>@media all{@font-face{font-family:'Fira Sans';font-style:normal;font-weight:400;src:url('https://fonts.gstatic.com/s/firasans/v17/va9E4kDNxMZdWfMOD5VvmYjN.woff') format('woff'),url('https://fonts.gstatic.com/s/firasans/v17/va9E4kDNxMZdWfMOD5VvmYjL.woff2') format('woff2')}@font-face{font-family:'Fira Sans';font-style:normal;font-weight:800;src:url('https://fonts.gstatic.com/s/firasans/v17/va9B4kDNxMZdWfMOD5VnMK7eSBf8.woff') format('woff'),url('https://fonts.gstatic.com/s/firasans/v17/va9B4kDNxMZdWfMOD5VnMK7eSBf6.woff2') format('woff2')}}</style></head><body class=\"body pc-font-alt\" style=\"width: 100% !important; min-height: 100% !important; margin: 0 !important; padding: 0 !important; line-height: 1.5; color: #2D3A41; mso-line-height-rule: exactly; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-variant-ligatures: normal; text-rendering: optimizeLegibility; -moz-osx-font-smoothing: grayscale; background-color: #f4f4f4;\" bgcolor=#f4f4f4><table class=pc-project-body style=\"table-layout: fixed; min-width: 600px;\" width=100% border=0 cellspacing=0 cellpadding=0 role=presentation><tr><td align=center valign=top><table class=pc-project-container style=\"width: 600px; max-width: 600px;\" width=600 align=center border=0 cellpadding=0 cellspacing=0 role=presentation><tr><td style=\"padding: 20px 0px 20px 0px;\" align=left valign=top><table border=0 cellpadding=0 cellspacing=0 role=presentation width=100% style=\"width: 100%;\"><tr><td valign=top><table width=100% border=0 cellspacing=0 cellpadding=0 role=presentation><tr><td style=\"padding: 0px 0px 0px 0px;\"><table width=100% border=0 cellspacing=0 cellpadding=0 role=presentation><tr><td valign=top class=\"pc-w520-padding-25-25-25-25 pc-w620-padding-30-30-30-30\" style=\"padding: 40px 40px 20px 40px; border-radius: 0px; background-color: #ffffff;\" bgcolor=#ffffff><table width=100% border=0 cellpadding=0 cellspacing=0 role=presentation><tr><td align=center valign=top style=\"padding: 0px 0px 21px 0px;\"><img src=\"https://buydirectfromauthors.com/assets/images/image01.png?v=cf190c0d\" width=125 height=25 alt=\"\" style=\"display: block; border: 0; outline: 0; line-height: 100%; -ms-interpolation-mode: bicubic; width:125px; height: auto; max-width: 100%;\" /></td></tr></table></td></tr></table></td></tr></table></td></tr><tr><td valign=top><table width=100% border=0 cellspacing=0 cellpadding=0 role=presentation><tr><td style=\"padding: 0px 0px 0px 0px;\"><table width=100% border=0 cellspacing=0 cellpadding=0 role=presentation><tr><td valign=top class=\"pc-w520-padding-30-30-30-30 pc-w620-padding-35-35-35-35\" style=\"padding: 20px 40px 20px 40px; border-radius: 0px; background-color: #ffffff;\" bgcolor=#ffffff><table width=100% border=0 cellpadding=0 cellspacing=0 role=presentation><tr><td align=left valign=top style=\"padding: 0px 0px 15px 0px;\"><table border=0 cellpadding=0 cellspacing=0 role=presentation width=100% style=\"border-collapse: separate; border-spacing: 0; margin-right: auto; margin-left: auto;\"><tr><td valign=top align=left><div class=\"pc-font-alt pc-w620-fontSize-30 pc-w620-lineHeight-133pc\" style=\"line-height: 128%; letter-spacing: -0.6px; font-family: 'Fira Sans', Arial, Helvetica, sans-serif; font-size: 24px; font-weight: 800; font-variant-ligatures: normal; color: #151515; text-align: left; text-align-last: left;\"><div style=\"text-align: -webkit-center; text-align-last: -webkit-center;\"><span style=\"font-weight: 400;font-style: normal;color: rgb(0, 0, 0);\">Here’s the login code you requested</span></div></div></td></tr></table></td></tr></table><table width=100% border=0 cellpadding=0 cellspacing=0 role=presentation><tr><td align=left valign=top style=\"padding: 0px 0px 15px 0px;\"><table border=0 cellpadding=0 cellspacing=0 role=presentation width=100% style=\"border-collapse: separate; border-spacing: 0; margin-right: auto; margin-left: auto;\"><tr><td valign=top align=left><div class=\"pc-font-alt pc-w620-fontSize-30 pc-w620-lineHeight-133pc\" style=\"line-height: 128%; letter-spacing: -0.6px; font-family: 'Fira Sans', Arial, Helvetica, sans-serif; font-size: 32px; font-weight: 800; font-variant-ligatures: normal; color: #151515; text-align: left; text-align-last: left;\"><div style=\"text-align: -webkit-center; text-align-last: -webkit-center;\"><span style=\"font-weight: 400;font-style: normal;color: rgb(0, 0, 0);\">######</span></div></div></td></tr></table></td></tr></table><table width=100% border=0 cellpadding=0 cellspacing=0 role=presentation><tr><td align=left valign=top style=\"padding: 0px 0px 20px 0px;\"><table border=0 cellpadding=0 cellspacing=0 role=presentation width=100% style=\"border-collapse: separate; border-spacing: 0; margin-right: auto; margin-left: auto;\"><tr><td valign=top align=left><div class=\"pc-font-alt pc-w620-fontSize-18 pc-w620-lineHeight-156pc\" style=\"line-height: 150%; letter-spacing: -0.2px; font-family: 'Fira Sans', Arial, Helvetica, sans-serif; font-size: 16px; font-weight: normal; font-variant-ligatures: normal; color: #9b9b9b; text-align: left; text-align-last: left;\"><div><span>Do not share this code with anyone. This code will expire in 15 mins.</span></div></div></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table><div class=pc-gmail-fix style=\"white-space: nowrap; font: 15px courier; line-height: 0;\">                                                           </div></body></html>");
            emailBody.Replace("######", _authToken);

            return emailBody.ToString();
        }
    }
}

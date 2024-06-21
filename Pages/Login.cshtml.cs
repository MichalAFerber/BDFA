using BDFA.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Web.WebPages;

namespace BDFA.Pages
{
    public class LoginModel : PageModel
    {
        private readonly BDFAdbContext _dbContext;

        public LoginModel(BDFAdbContext db)
        {
            _dbContext = db;
        }

        public bool ShowSendCode { get; set; } = true;

        public bool ShowSignIn { get; set; } = false;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; } = false;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            if (Email.IsEmpty())
            {
                return Page();
            }

            BL.Manager.SendMail(Email, "Login Code", GenerateEmail());

            ShowSignIn = true;
            ShowSendCode = true;
            await _dbContext.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSignInAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            BL.Manager.SendMail(Email, "Login Code", GenerateEmail());
            Email = "WORKS";
            return RedirectToPage("/Profile");
        }

        private static string GenerateOneTimeCode()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }

        private static string GenerateEmail()
        {
            StringBuilder emailBody = new();
            emailBody.Append("Your one-time code is: ");
            emailBody.Append(GenerateOneTimeCode());
            emailBody.Append("<br><br>Thank you for using our service.");

            return emailBody.ToString();
        }
    }
}

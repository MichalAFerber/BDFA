using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPostSendCode()
        {
            // Query BDFAdb admins table to check if email exists
            bool emailExists = CheckIfEmailExists(Email);

            if (!emailExists)
            {
                // Generate unique 6-digit one-time code
                string code = GenerateOneTimeCode();

                // Send email with the code
                Manager.SendEmail(Email, "Login code requested for the Buy Direct From Authors website.", GenerateOneTimeCode());

                // Return success message
                return Content("Code sent successfully");
            }
            else
            {

                // Return success message
                return Content("Code sent successfully");
            }
        }

        private bool CheckIfEmailExists(string? email)
        {
            // Query BDFAdb admins table to check if email exists
            return BDFAdbContext.admins.Any(a => a.Email == email);
        }

        private string GenerateOneTimeCode()
        {
            // Generate unique 6-digit one-time code
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}

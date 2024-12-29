using Microsoft.AspNetCore.Mvc.RazorPages;
namespace BDFA.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        // Constructor to initialize the logger
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        // Method to handle GET requests
        public void OnGet()
        {
            // Set the title of the page if not already set
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Privacy Policy - Buy Direct From Authors";
            // Set the body title of the page if not already set
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Privacy Policy";
        }
    }

}

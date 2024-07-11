using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Privacy Policy - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Privacy Policy";
        }
    }

}

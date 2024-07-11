using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    public class TermsModel : PageModel
    {
        private readonly ILogger<TermsModel> _logger;

        public TermsModel(ILogger<TermsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Terms & Conditions - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Terms & Conditions";
        }
    }
}

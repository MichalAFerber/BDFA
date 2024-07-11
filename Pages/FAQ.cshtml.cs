using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    public class FAQModel : PageModel
    {
        private readonly ILogger<FAQModel> _logger;

        public FAQModel(ILogger<FAQModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "FAQs - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "FAQs";
        }
    }
}

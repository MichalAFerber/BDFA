using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    /// <summary>
    /// The TermsModel class represents the model for the Terms & Conditions page.
    /// </summary>
    public class TermsModel : PageModel
    {
        private readonly ILogger<TermsModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsModel"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to log information.</param>
        public TermsModel(ILogger<TermsModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles GET requests to the Terms & Conditions page.
        /// </summary>
        public void OnGet()
        {
            // Set the page title and body title if they are not already set.
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Terms & Conditions - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Terms & Conditions";
        }
    }
}

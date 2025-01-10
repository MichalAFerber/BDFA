using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BDFA.Pages
{
    /// <summary>
    /// The TermsModel class represents the model for the Terms & Conditions page.
    /// </summary>
    public class TermsModel : PageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermsModel"/> class.
        /// </summary>
        public TermsModel()
        {
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

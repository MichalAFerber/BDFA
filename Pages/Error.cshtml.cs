using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace BDFA.Pages
{
    // Disable response caching for this page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // Ignore the antiforgery token validation for this page
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        // Property to store the request ID
        public string RequestId { get; set; }

        // Property to determine if the request ID should be shown
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Logger instance for logging purposes
        private readonly ILogger<ErrorModel> _logger;

        // Constructor to initialize the logger
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        // Method to handle GET requests
        public void OnGet()
        {
            // Set the page title if not already set
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Error - Buy Direct From Authors";
            // Set the body title if not already set
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "";

            // Get the current activity ID or the HTTP context trace identifier
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}

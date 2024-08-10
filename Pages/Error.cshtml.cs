using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace BDFA.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]

    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Error - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "";

            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}

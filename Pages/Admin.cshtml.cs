using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ILogger<AdminModel> _logger;
        private readonly DirectoryContext _context;

        public IList<Profile> Profiles { get; set; }

        public AdminModel(ILogger<AdminModel> logger, DirectoryContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Admin Dashboard - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Admin Dashboard";

            var _isAuth = HttpContext.Session.GetInt32("IsAuth");
            var _isAdmin = HttpContext.Session.GetInt32("IsAdmin");
            var _email = HttpContext.Session.GetString("EmailKey");

            if ((_isAuth == null || _isAuth == 0) || (_isAdmin == null || _isAdmin == 0))
            {
                return RedirectToPage("./Login");
            }
            else
            {
                int idParam = Convert.ToInt32(Request.Query["id"]);
                string functionParam = Request.Query["function"];
                int functionStatus = Convert.ToInt32(Request.Query["status"]);

                // Handle different cases based on the function query string parameter
                switch (functionParam)
                {
                    case "status":
                        BL.Manager.ChangeProfileStatus(idParam, Convert.ToBoolean(functionStatus));
                        break;
                    case "featured":
                        BL.Manager.ChangeFeaturedStatus(idParam, Convert.ToBoolean(functionStatus));
                        break;
                    case "delete":
                        BL.Manager.DeleteProfile(idParam);
                        break;
                    default:
                        break;
                }

                Profiles = await _context.Profiles
                                         .OrderBy(p => p.Author)
                                         .ToListAsync();

                return Page();
            }
        }
    }
}

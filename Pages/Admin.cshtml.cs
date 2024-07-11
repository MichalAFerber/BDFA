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

            if ((_isAuth == null || _isAuth == 0) || (_isAdmin == null || _isAdmin == 0))
            {
                return RedirectToPage("./Login");
            }
            else
            {
                Profiles = await _context.Profiles.ToListAsync();
                return Page();
            }
        }
    }
}

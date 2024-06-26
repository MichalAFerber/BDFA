using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BDFA.Pages
{
    public class AdminModel : PageModel
    {
        private readonly BDFA.Data.DirectoryContext _context;
        public AdminModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        // Property to hold the list of profiles
        public IList<Profile> Profiles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = ViewData["Title"] ?? "Admin";

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

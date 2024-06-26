using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public byte[] Image { get; set; }
        [BindProperty]
        public string Author { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Tagline { get; set; }
        [BindProperty]
        public string Tags { get; set; }
        [BindProperty]
        public string UrlStore { get; set; }
        [BindProperty]
        public string UrlNewsletter { get; set; }
        [BindProperty]
        public string UrlFBGroup { get; set; }
        [BindProperty]
        public string UrlFBPage { get; set; }
        [BindProperty]
        public string UrlIG { get; set; }
        [BindProperty]
        public string UrlTikTok { get; set; }
        [BindProperty]
        public string UrlThreads { get; set; }
        [BindProperty]
        public string UrlX { get; set; }
        [BindProperty]
        public string UrlOther { get; set; }

        public Profile Profile { get; set; }

        private readonly BDFA.Data.DirectoryContext _context;
        public ProfileModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = ViewData["Title"] ?? "My Profile";

            var _isAuth = HttpContext.Session.GetInt32("IsAuth");
            var _email = HttpContext.Session.GetString("EmailKey");

            if (_isAuth == 0 || _email == null)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                Profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == _email);

                if (Profile == null)
                {
                    return NotFound();
                }

                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var _email = HttpContext.Session.GetString("EmailKey");

            var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == _email);

            if (existingProfile == null)
            {
                // If no existing profile is found, add a new one
                // Instantiate the Profile object
                Profile = new Profile
                {
                    // Populate the Profile properties with data
                    Email = "New Email",
                    AuthToken = "00000",
                };
                // Now, you can add the instantiated Profile to the database context
                _context.Profiles.Add(Profile);
            }
            else
            {
                existingProfile.Author = Author;
                existingProfile.Image = Image;
                existingProfile.Email = Email;
                existingProfile.Tagline = Tagline;
                existingProfile.Tags = Tags;
                existingProfile.UrlStore = UrlStore;
                existingProfile.UrlNewsletter = UrlNewsletter;
                existingProfile.UrlFBGroup = UrlFBGroup;
                existingProfile.UrlFBPage = UrlFBPage;
                existingProfile.UrlIG = UrlIG;
                existingProfile.UrlTikTok = UrlTikTok;
                existingProfile.UrlThreads = UrlThreads;
                existingProfile.UrlX = UrlX;
                existingProfile.UrlOther = UrlOther;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return Page(); // Or handle the case where the profile doesn't exist
        }
    }
}
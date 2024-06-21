using BDFA.BL;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BDFA.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public string About { get; set; } = "This is where you can add a short bio about yourself. You can also add links to your store, newsletter, social media, and other platforms.";
        [BindProperty]
        public int ID { get; set; }
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

        [BindProperty]
        public Profile Profile { get; set; } = default!;

        private readonly BDFA.Data.DirectoryContext _context;

        public ProfileModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            About = Manager.SettingsProfileAbout;
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if the email already exists in the database
            var existingProfile = await _context.Profiles
                                                .FirstOrDefaultAsync(p => p.Email == Profile.Email);
            if (existingProfile != null)
            {
                // Add a model error on the Email field
                ModelState.AddModelError("Profile.Email", "The email address already exists.");
                return Page();
            }

            _context.Profiles.Add(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
using BDFA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        [BindProperty]
        public string Image { get; set; }
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
        public string UrlReam { get; set; }
        [BindProperty]
        public string UrlPatreon { get; set; }
        [BindProperty]
        public string UrlYouTube { get; set; }
        [BindProperty]
        public string UrlOther { get; set; }

        // Add a property to store the filename
        public string ImageFilename { get; set; }

        private readonly ILogger<ProfileModel> _logger;
        private readonly DirectoryContext _context;
        private readonly IWebHostEnvironment _hostenvironment;

        public ProfileModel(ILogger<ProfileModel> logger, DirectoryContext context, IWebHostEnvironment hostenvironment)
        {
            _logger = logger;
            _context = context;
            _hostenvironment = hostenvironment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "My Profile - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "My Profile";

            var _isAuth = HttpContext.Session.GetInt32("IsAuth");
            var _email = HttpContext.Session.GetString("EmailKey");

            if (_isAuth == 0 || _email == null)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                // Fetch the Profile object from the database
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == _email);

                // Log the result of the query
                if (profile == null)
                {
                    return NotFound("Profile not found.");
                }

                // Populate the bound properties with the profile data
                Image = profile.Image;
                Author = profile.Author;
                Email = _email;
                Tagline = profile.Tagline;
                Tags = profile.Tags;
                UrlStore = profile.UrlStore;
                UrlNewsletter = profile.UrlNewsletter;
                UrlFBGroup = profile.UrlFBGroup;
                UrlFBPage = profile.UrlFBPage;
                UrlIG = profile.UrlIG;
                UrlTikTok = profile.UrlTikTok;
                UrlThreads = profile.UrlThreads;
                UrlX = profile.UrlX;
                UrlReam = profile.UrlReam;
                UrlPatreon = profile.UrlPatreon;
                UrlOther = profile.UrlOther;
                UrlYouTube = profile.UrlYouTube;

                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the email from the session
            var email = HttpContext.Session.GetString("EmailKey");
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is not provided.");
            }

            // Fetch the Profile object from the database
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == email);

            if (profile == null)
            {
                return NotFound("Profile not found.");
            }

            // Handle file upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Generate a unique filename based on the current date and time
                var filename = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(ImageFile.FileName.ToLower())}";
                var filePath = Path.Combine(_hostenvironment.WebRootPath, "i", filename);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Update the Profile entity with the filename
                profile.Image = filename;
            }

            // Update other properties
            profile.Author = Author;
            profile.Tagline = Tagline;
            profile.Tags = Tags;
            profile.UrlStore = UrlStore;
            profile.UrlNewsletter = UrlNewsletter;
            profile.UrlFBGroup = UrlFBGroup;
            profile.UrlFBPage = UrlFBPage;
            profile.UrlIG = UrlIG;
            profile.UrlTikTok = UrlTikTok;
            profile.UrlThreads = UrlThreads;
            profile.UrlX = UrlX;
            profile.UrlReam = UrlReam;
            profile.UrlPatreon = UrlPatreon;
            profile.UrlOther = UrlOther;
            profile.UrlYouTube = UrlYouTube;

            // Mark the Profile entity as modified
            _context.Attach(profile).State = EntityState.Modified;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profile");
        }
    }
}
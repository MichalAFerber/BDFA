using BDFA.BL;
using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Data;
using System.Runtime.InteropServices;

namespace BDFA.Pages
{
    /// <summary>
    /// The ProfileModel class represents the model for the Profile page.
    /// </summary>
    public class ProfileModel : PageModel
    {
        #region Properties
        // Properties are defined here
        [BindProperty]
        public string fStartDate { get; set; }
        [BindProperty]
        public string fEndDate { get; set; }
        [BindProperty]
        public string fClickedLink { get; set; }
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
        [BindProperty]

        public string ImageFilename { get; set; } // Add a property to store the filename

        public int isAuth { get; set; } = 0;
        public string pEmail { get; set; } = string.Empty;
        public int pId { get; set; } = 0;
        #endregion

        private readonly DirectoryContext _context;
        private readonly ILogger<ProfileModel> _logger;
        private readonly IWebHostEnvironment _hostenvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileModel"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to log information.</param>
        /// <param name="context">The database context.</param>
        /// <param name="hostenvironment">The web host environment.</param>
        public ProfileModel(ILogger<ProfileModel> logger, DirectoryContext context, IWebHostEnvironment hostenvironment)
        {
            _context = context;
            _logger = logger;
            _hostenvironment = hostenvironment;
        }

        /// <summary>
        /// Handles GET requests to the Profile page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "My Profile - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "My Profile";

            if (!Globals.isAuth)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                if (Globals.pId < 0)
                {
                    if (!StringValues.IsNullOrEmpty(Request.Query["id"]))
                    {
                        Globals.pId = Convert.ToInt32(Request.Query["id"]);
                    }
                }

                Profile profile = null;
                profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == Globals.pId);

                if (profile == null)
                {
                    return NotFound();
                }
                else
                {
                    // Populate the bound properties with the profile data
                    Image = profile.Image;
                    Author = profile.Author;
                    Email = profile.Email;
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
        }

        /// <summary>
        /// Handles POST requests to the Profile page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string _tab = Request.Query["tab"];
            string _function = Request.Query["function"];

            if (_tab == "Stats")
            {
                await GetClickDataAsync(Globals.pId, fClickedLink, fStartDate, fEndDate);
            }

            if (_tab == "Edit")
            {
                await SaveProfile();
            }

            return Page();
        }

        /// <summary>
        /// Retrieves click data for the specified profile.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="clickedLink">The clicked link to filter by (optional).</param>
        /// <param name="startDate">The start date to filter by (optional).</param>
        /// <param name="endDate">The end date to filter by (optional).</param>
        /// <returns>A list of <see cref="ClickDataGroup"/> objects representing the click data.</returns>
        public async Task<List<ClickDataGroup>> GetClickDataAsync(int profileId, [Optional] string clickedLink, [Optional] string startDate, [Optional] string endDate)
        {
            var query = _context.Clicks.AsQueryable();

            query = query.Where(cd => cd.ProfileId == profileId);

            if (!string.IsNullOrEmpty(clickedLink))
            {
                if (clickedLink == "0")
                {
                    clickedLink = "0";
                }
                else
                {
                    query = query.Where(cd => cd.Link == clickedLink);
                }
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime startDateTime = Convert.ToDateTime(startDate);
                DateTime endDateTime = Convert.ToDateTime(endDate);

                query = query.Where(cd => cd.ClickDateTime >= startDateTime && cd.ClickDateTime <= endDateTime);
            }

            var groupedData = await query
                .GroupBy(cd => cd.Link)
                .Select(g => new ClickDataGroup
                {
                    Link = g.Key,
                    ClickCount = g.Count()
                })
                .ToListAsync();

            return groupedData;
        }

        /// <summary>
        /// Retrieves distinct links for the current profile.
        /// </summary>
        /// <returns>A list of distinct links.</returns>
        public async Task<List<string>> GetDistinctLinksAsync()
        {
            var distinctLinks = await _context.Clicks
                .Where(cd => cd.ProfileId == Globals.pId)
                .Select(cd => cd.Link)
                .Distinct()
                .OrderBy(link => link)
                .ToListAsync();

            return distinctLinks;
        }

        /// <summary>
        /// Saves the profile data to the database.
        /// </summary>
        private async Task SaveProfile()
        {
            var _email = HttpContext.Session.GetString("EmailKey");
            Profile profile = null;

            // Fetch the Profile object from the database
            profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == Globals.pId);

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
        }
    }
}
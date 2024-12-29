using BDFA.BL;
using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BDFA.Pages
{
    public class AdminModel : PageModel
    {
        #region Properties
        [BindProperty]
        public string fStartDate { get; set; }
        [BindProperty]
        public string fEndDate { get; set; }
        [BindProperty]
        public string fClickedLink { get; set; }
        [BindProperty]
        public IFormFile Image1File { get; set; }
        [BindProperty]
        public IFormFile Image2File { get; set; }
        [BindProperty]
        public IFormFile Image3File { get; set; }
        [BindProperty]
        public string DealImage1 { get; set; }
        [BindProperty]
        public string DealImage2 { get; set; }
        [BindProperty]
        public string DealImage3 { get; set; }
        [BindProperty]
        public string DealURL1 { get; set; }
        [BindProperty]
        public string DealURL2 { get; set; }
        [BindProperty]
        public string DealURL3 { get; set; }
        #endregion

        // Logger for logging information and errors
        private readonly ILogger<AdminModel> _logger;
        // Database context for accessing the database
        private readonly DirectoryContext _context;
        // Environment for accessing web hosting information
        private readonly IWebHostEnvironment _hostenvironment;
        // Configuration for accessing configuration settings
        private readonly IConfiguration _configuration;

        // List of profiles to be displayed on the page
        public IList<Profile> Profiles { get; set; }

        // Constructor to initialize the AdminModel with dependencies
        public AdminModel(ILogger<AdminModel> logger, DirectoryContext context, IWebHostEnvironment hostenvironment, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _hostenvironment = hostenvironment;
            _configuration = configuration;
        }

        // Method to handle GET requests
        public async Task<IActionResult> OnGetAsync()
        {
            // Set the title for the page and body
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Admin Dashboard - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Admin Dashboard";

            // Check if the user is authenticated and is an admin
            if (!Globals.isAuth || !Globals.isAdmin)
            {
                // Redirect to the login page if not authenticated or not an admin
                return RedirectToPage("./Login");
            }
            else
            {
                // Retrieve the site ID from the configuration
                var _ConfigSiteID = Convert.ToInt32(_configuration["Settings:SiteID"]);
                // Reset the profile ID
                Globals.pId = -1;

                // Retrieve query parameters
                string functionParam = Request.Query["function"];
                int functionStatus = Convert.ToInt32(Request.Query["status"]);
                int _Id = Convert.ToInt32(Request.Query["id"]);

                if (!_Id.Equals(null))
                {
                    // Handle different cases based on the function query string parameter
                    switch (functionParam)
                    {
                        case "status":
                            // Change the profile status
                            await Manager.ChangeProfileStatusAsync(_Id, Convert.ToBoolean(functionStatus));
                            break;
                        case "featured":
                            // Change the featured status of the profile
                            await Manager.ChangeFeaturedStatusAsync(_Id, Convert.ToBoolean(functionStatus));
                            break;
                        case "delete":
                            // Delete the profile
                            await Manager.DeleteProfileAsync(_Id);
                            break;
                        case "deleteDeal":
                            // Set values NULL for the deal image and URL
                            await Manager.DeleteDealAsync(_Id, _ConfigSiteID);
                            // Retrieve the updated site settings
                            await Manager.GetSiteSettingsAsync(_ConfigSiteID);
                            break;
                        default:
                            break;
                    }
                }

                // Get all profiles and order them by author
                Profiles = await _context.Profiles
                                         .OrderBy(p => p.Author)
                                         .ToListAsync();

                // Get all settings
                Setting siteSettings = null;
                siteSettings = await _context.Settings.FirstOrDefaultAsync(p => p.ID == _ConfigSiteID);

                if (siteSettings == null)
                {
                    // Return not found if site settings are not found
                    return NotFound("Site settings not found.");
                }

                // Populate the bound properties with the profile data
                DealImage1 = siteSettings.DealImage1;
                DealURL1 = siteSettings.DealURL1;
                DealImage2 = siteSettings.DealImage2;
                DealURL2 = siteSettings.DealURL2;
                DealImage3 = siteSettings.DealImage3;
                DealURL3 = siteSettings.DealURL3;

                // Return the page
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve query parameters
            string _tab = Request.Query["tab"];
            string _function = Request.Query["function"];

            // If the tab is "Profiles" or not specified, get all profiles
            if (_tab == "Profiles" || _tab == null || _tab == string.Empty)
            {
                Profiles = await _context.Profiles
                                         .OrderBy(p => p.Author)
                                         .ToListAsync();
            }

            // If the tab is "Settings", handle settings update
            if (_tab == "Settings")
            {
                // Retrieve the site ID from the appsettings.json file
                var _ConfigSiteID = Convert.ToInt32(_configuration["Settings:SiteID"]);

                // Retrieve the site settings from the database
                Setting siteSettings = await _context.Settings.FirstOrDefaultAsync(p => p.ID == _ConfigSiteID);

                if (siteSettings == null)
                {
                    return NotFound("Site settings not found.");
                }

                // Handle file upload for deal image 1
                if (Image1File != null && Image1File.Length > 0)
                {
                    // Generate a unique filename based on the current date and time
                    var filename1 = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(Image1File.FileName.ToLower())}";
                    var filePath1 = Path.Combine(_hostenvironment.WebRootPath, "i", filename1);

                    // Save the file to the specified path
                    using (var fileStream1 = new FileStream(filePath1, FileMode.Create))
                    {
                        await Image1File.CopyToAsync(fileStream1);
                    }

                    // Update the Setting entity with the filename
                    siteSettings.DealImage1 = filename1;
                }

                // Handle file upload for deal image 2
                if (Image2File != null && Image2File.Length > 0)
                {
                    // Generate a unique filename based on the current date and time
                    var filename2 = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(Image2File.FileName.ToLower())}";
                    var filePath2 = Path.Combine(_hostenvironment.WebRootPath, "i", filename2);

                    // Save the file to the specified path
                    using (var fileStream2 = new FileStream(filePath2, FileMode.Create))
                    {
                        await Image2File.CopyToAsync(fileStream2);
                    }

                    // Update the Setting entity with the filename
                    siteSettings.DealImage2 = filename2;
                }

                // Handle file upload for deal image 3
                if (Image3File != null && Image3File.Length > 0)
                {
                    // Generate a unique filename based on the current date and time
                    var filename3 = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(Image3File.FileName.ToLower())}";
                    var filePath3 = Path.Combine(_hostenvironment.WebRootPath, "i", filename3);

                    // Save the file to the specified path
                    using (var fileStream3 = new FileStream(filePath3, FileMode.Create))
                    {
                        await Image3File.CopyToAsync(fileStream3);
                    }

                    // Update the Setting entity with the filename
                    siteSettings.DealImage3 = filename3;
                }

                // Update other properties
                siteSettings.DealURL1 = DealURL1;
                siteSettings.DealURL2 = DealURL2;
                siteSettings.DealURL3 = DealURL3;

                // Mark the Setting entity as modified
                _context.Attach(siteSettings).State = EntityState.Modified;

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Retrieve the updated site settings
                await Manager.GetSiteSettingsAsync(_ConfigSiteID);
            }

            // If the tab is "Stats", get click data
            if (_tab == "Stats")
            {
                await GetClickDataAsync(fClickedLink, fStartDate, fEndDate);
            }

            return Page();
        }

        public async Task<List<ProfileDataGroup>> GetProfileDataAsync()
        {
            // Create a queryable object for Profiles
            var query = _context.Profiles.AsQueryable();

            // Group the data by author and email
            var groupedData = await query
                .GroupBy(cd => new { cd.Author, cd.Email })
                .Select(g => new ProfileDataGroup
                {
                    Author = g.Key.Author,
                    Email = g.Key.Email,
                })
                .ToListAsync();

            return groupedData;
        }

        public async Task<List<string>> GetDistinctProfilesAsync()
        {
            // Get distinct authors from the Profiles table
            var distinctProfiles = await _context.Profiles
                .Select(cd => cd.Author)
                .Distinct()
                .ToListAsync();

            return distinctProfiles;
        }

        public async Task<List<ClickDataGroup>> GetClickDataAsync([Optional] string clickedLink, [Optional] string startDate, [Optional] string endDate)
        {
            // Create a queryable object for Clicks
            var query = _context.Clicks.AsQueryable();

            // Filter by clicked link if provided
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

            // Filter by date range if both start and end dates are provided
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime startDateTime = Convert.ToDateTime(startDate);
                DateTime endDateTime = Convert.ToDateTime(endDate);

                query = query.Where(cd => cd.ClickDateTime >= startDateTime && cd.ClickDateTime <= endDateTime);
            }

            // Group the data by link and count the clicks
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

        public async Task<List<ClickDataGroup>> GetClickDataAsync(int profileId, [Optional] string clickedLink, [Optional] string startDate, [Optional] string endDate)
        {
            // Create a queryable object for Clicks
            var query = _context.Clicks.AsQueryable();

            // Filter by profile ID
            query = query.Where(cd => cd.ProfileId == profileId);

            // Filter by clicked link if provided
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

            // Filter by date range if both start and end dates are provided
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime startDateTime = Convert.ToDateTime(startDate);
                DateTime endDateTime = Convert.ToDateTime(endDate);

                query = query.Where(cd => cd.ClickDateTime >= startDateTime && cd.ClickDateTime <= endDateTime);
            }

            // Group the data by link and count the clicks
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

        public async Task<List<string>> GetDistinctLinksAsync()
        {
            // Get distinct links from the Clicks table and order them
            var distinctLinks = await _context.Clicks
                .Select(cd => cd.Link)
                .Distinct()
                .OrderBy(link => link)
                .ToListAsync();

            return distinctLinks;
        }
    }
}
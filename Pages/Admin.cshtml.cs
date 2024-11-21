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

        private readonly ILogger<AdminModel> _logger;
        private readonly DirectoryContext _context;
        private readonly IWebHostEnvironment _hostenvironment;
        private readonly IConfiguration _configuration;

        public IList<Profile> Profiles { get; set; }

        public AdminModel(ILogger<AdminModel> logger, DirectoryContext context, IWebHostEnvironment hostenvironment, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _hostenvironment = hostenvironment;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Admin Dashboard - Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? "Admin Dashboard";

            if (!Globals.isAuth || !Globals.isAdmin)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                var _ConfigSiteID = Convert.ToInt32(_configuration["Settings:SiteID"]);
                Globals.pId = -1; // Reset the profile ID

                string functionParam = Request.Query["function"];
                int functionStatus = Convert.ToInt32(Request.Query["status"]);
                int _Id = Convert.ToInt32(Request.Query["id"]);

                if (!_Id.Equals(null))
                {
                    // Handle different cases based on the function query string parameter
                    switch (functionParam)
                    {
                        case "status":
                            Manager.ChangeProfileStatus(_Id, Convert.ToBoolean(functionStatus));
                            break;
                        case "featured":
                            Manager.ChangeFeaturedStatus(_Id, Convert.ToBoolean(functionStatus));
                            break;
                        case "delete":
                            Manager.DeleteProfile(_Id);
                            break;
                        case "deleteDeal":
                            // Set values NULL for the deal image and URL
                            Manager.DeleteDeal(_Id, _ConfigSiteID);
                            // Retrieve the updated site settings
                            Manager.GetSiteSettings(_ConfigSiteID);
                            break;
                        default:
                            break;
                    }
                }

                // Get all profiles
                Profiles = await _context.Profiles
                                         .OrderBy(p => p.Author)
                                         .ToListAsync();

                // Get all settings
                Setting siteSettings = null;
                siteSettings = await _context.Settings.FirstOrDefaultAsync(p => p.ID == _ConfigSiteID);

                if (siteSettings == null)
                {
                    return NotFound("Site settings not found.");
                }

                // Populate the bound properties with the profile data
                DealImage1 = siteSettings.DealImage1;
                DealURL1 = siteSettings.DealURL1;
                DealImage2 = siteSettings.DealImage2;
                DealURL2 = siteSettings.DealURL2;
                DealImage3 = siteSettings.DealImage3;
                DealURL3 = siteSettings.DealURL3;

                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string _tab = Request.Query["tab"];
            string _function = Request.Query["function"];

            if(_tab == "Profiles" || _tab == null || _tab == string.Empty)
            {
                // Get all profiles
                Profiles = await _context.Profiles
                                         .OrderBy(p => p.Author)
                                         .ToListAsync();
            }

            if (_tab == "Settings")
            {
                // Retrieve the site ID from the appsettings.json file
                var _ConfigSiteID = Convert.ToInt32(_configuration["Settings:SiteID"]);

                Setting siteSettings = null;
                siteSettings = await _context.Settings.FirstOrDefaultAsync(p => p.ID == _ConfigSiteID);

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
                Manager.GetSiteSettings(_ConfigSiteID);
            }

            if (_tab == "Stats")
            {
                await GetClickDataAsync(fClickedLink, fStartDate, fEndDate);
            }

            return Page();
        }

        public async Task<List<ProfileDataGroup>> GetProfileDataAsync()
        {
            var query = _context.Profiles.AsQueryable();

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
            var distinctProfiles = await _context.Profiles
                .Select(cd => cd.Author)
                .Distinct()
                .ToListAsync();

            return distinctProfiles;
        }

        public async Task<List<ClickDataGroup>> GetClickDataAsync([Optional] string clickedLink, [Optional] string startDate, [Optional] string endDate)
        {
            var query = _context.Clicks.AsQueryable();

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

        public async Task<List<string>> GetDistinctLinksAsync()
        {
            var distinctLinks = await _context.Clicks
                .Select(cd => cd.Link)
                .Distinct()
                .OrderBy(link => link)
                .ToListAsync();

            return distinctLinks;
        }
    }
}
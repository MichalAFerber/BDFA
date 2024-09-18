using BDFA.BL;
using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class AdminModel : PageModel
    {
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

            var _ConfigSiteID = Convert.ToInt32(_configuration["Settings:SiteID"]);

            if (!Globals.isAuth || !Globals.isAdmin)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                string functionParam = Request.Query["function"];
                int functionStatus = Convert.ToInt32(Request.Query["status"]);

                // Handle different cases based on the function query string parameter
                switch (functionParam)
                {
                    case "status":
                        BL.Manager.ChangeProfileStatus(Globals.pId, Convert.ToBoolean(functionStatus));
                        break;
                    case "featured":
                        BL.Manager.ChangeFeaturedStatus(Globals.pId, Convert.ToBoolean(functionStatus));
                        break;
                    case "delete":
                        BL.Manager.DeleteProfile(Globals.pId);
                        break;
                        case "deleteDeal":
                        // Set values NULL for the deal image and URL
                        BL.Manager.DeleteDeal(Globals.pId, _ConfigSiteID);
                        // Retrieve the updated site settings
                        BL.Manager.GetSiteSettings(_ConfigSiteID);
                        break;
                    default:
                        break;
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
            BL.Manager.GetSiteSettings(_ConfigSiteID);

            // Get all profiles
            Profiles = await _context.Profiles
                                     .OrderBy(p => p.Author)
                                     .ToListAsync();

            return RedirectToPage("./Admin");
        }
    }
}

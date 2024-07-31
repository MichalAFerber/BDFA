using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DirectoryContext _context;

        public IList<Profile> FeaturedAuthors { get; set; }
        public IList<Profile> Profiles { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DirectoryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Buy Direct From Authors";
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            // Get all featured authors and active profiles
            FeaturedAuthors = await _context.Profiles
                .Where(p => p.Active && p.FeaturedAuthor)
                .OrderBy(p => p.Author)
                .ToListAsync();

            // Get all active profiles
            Profiles = await _context.Profiles
                .Where(p => p.Active)
                .OrderBy(p => p.RowId)
                .ToListAsync();

            return Page();
        }

        public IActionResult OnGetProfilesPartial(string searchQuery)
        {
            if (Profiles == null)
            {
                // Fetch profiles if they are not already loaded
                Profiles = _context.Profiles
                    .Where(p => p.Active)
                    .OrderByDescending(p => p.RowId)
                    .ToList();
            }

            var filteredProfiles = string.IsNullOrEmpty(searchQuery)
                ? Profiles
                : Profiles.Where(p => p.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                   || p.Tagline.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                   || p.Tags.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                   )
                          .ToList();

            return Partial("_ProfilesList", filteredProfiles);
        }

    }
}

using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DirectoryContext _context;

        public IndexModel(DirectoryContext context)
        {
            _context = context;
        }

        public IList<Profile> FeaturedAuthors { get; set; }
        public IList<Profile> FeaturedDeals { get; set; }
        public IList<Profile> Profiles { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            // Get all featured authors and active profiles
            FeaturedAuthors = await _context.Profiles
                .Where(p => p.Active && p.FeaturedAuthor)
                .OrderBy(p => p.RowId)
                .ToListAsync();

            // Get all featured deals and active profiles
            FeaturedDeals = await _context.Profiles
                .Where(p => p.Active && p.FeaturedDeal)
                .OrderBy(p => p.RowId)
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
                    .OrderBy(p => p.RowId)
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

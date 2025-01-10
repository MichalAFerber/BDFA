using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, DirectoryContext context) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly DirectoryContext _context = context;

        // Properties to hold featured authors, deals, and profiles
        public IList<Profile> FeaturedAuthors { get; set; }
        public IList<string> FeaturedDeals { get; set; }
        public IList<Profile> Profiles { get; set; }

        // Method to handle GET requests asynchronously
        public async Task<IActionResult> OnGetAsync()
        {
            // Set the title of the page if not already set
            ViewData["TitlePage"] = ViewData["TitlePage"] ?? "Buy Direct From Authors";
            // Set the body title of the page if not already set
            ViewData["TitleBody"] = ViewData["TitleBody"] ?? string.Empty;

            // Get all featured authors and active profiles
            FeaturedAuthors = await _context.Profiles
                .Where(p => p.Active && p.FeaturedAuthor) // Filter active and featured authors
                .OrderBy(p => p.Author) // Order by author name
                .ToListAsync();

            // Get all featured deals from settings
            var settings = await _context.Settings.ToListAsync();
            FeaturedDeals = settings
                .SelectMany(s => new List<string> { s.DealURL1, s.DealURL2, s.DealURL3 }) // Collect all deal URLs
                .Where(url => !string.IsNullOrEmpty(url)) // Filter out empty URLs
                .ToList();

            // Get all active profiles
            Profiles = await _context.Profiles
                .Where(p => p.Active && p.Author.Length > 0) // Filter active profiles with non-empty author names
                .OrderBy(p => p.Id) // Order by profile ID
                .ToListAsync();

            return Page();
        }

        // Method to handle partial GET requests for profiles based on a search query
        public IActionResult OnGetProfilesPartial(string searchQuery)
        {
            // Fetch profiles if they are not already loaded
            Profiles ??= [.. _context.Profiles
                .Where(p => p.Active && p.Author.Length > 0) // Filter active profiles with non-empty author names
                .OrderBy(p => p.Id)];

            // Filter profiles based on the search query
            var filteredProfiles = string.IsNullOrEmpty(searchQuery)
                ? Profiles // If no search query, return all profiles
                : Profiles.Where(p => p.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) // Filter by author name
                                   || p.Tagline.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) // Filter by tagline
                                   || p.Tags.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) // Filter by tags
                                   )
                          .ToList();

            return Partial("_ProfilesList", filteredProfiles); // Return the filtered profiles as a partial view
        }
    }
}
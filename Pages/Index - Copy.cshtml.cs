using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Pages
{
    public class IndexModelcopy : PageModel
    {
        private readonly DirectoryContext _context;

        public IndexModelcopy(DirectoryContext context)
        {
            _context = context;
        }

        public IList<Profile> Profiles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Profiles = await _context.Profiles
                .Where(p => p.Active)
                .OrderBy(p => p.RowId)
                .ToListAsync();

            return Page();
        }
    }
}

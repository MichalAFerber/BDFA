using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BDFA.Data;
using BDFA.Models;

namespace BDFA.Pages.Directory
{
    public class DetailsModel : PageModel
    {
        private readonly BDFA.Data.DirectoryContext _context;

        public DetailsModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        public Profile Profile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(m => m.ID == id);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                Profile = profile;
            }
            return Page();
        }
    }
}

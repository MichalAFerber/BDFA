using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BDFA.Data;
using BDFA.Models;

namespace BDFA.Pages.SiteSettings
{
    public class DetailsModel : PageModel
    {
        private readonly BDFA.Data.SiteSettingsContext _context;

        public DetailsModel(BDFA.Data.SiteSettingsContext context)
        {
            _context = context;
        }

        public Setting Setting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Setting.FirstOrDefaultAsync(m => m.ID == id);
            if (setting == null)
            {
                return NotFound();
            }
            else
            {
                Setting = setting;
            }
            return Page();
        }
    }
}

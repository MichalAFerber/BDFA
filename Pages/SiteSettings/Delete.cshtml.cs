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
    public class DeleteModel : PageModel
    {
        private readonly BDFA.Data.SiteSettingsContext _context;

        public DeleteModel(BDFA.Data.SiteSettingsContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Setting.FindAsync(id);
            if (setting != null)
            {
                Setting = setting;
                _context.Setting.Remove(Setting);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

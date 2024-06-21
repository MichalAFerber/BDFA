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
    public class IndexModel : PageModel
    {
        private readonly BDFA.Data.SiteSettingsContext _context;

        public IndexModel(BDFA.Data.SiteSettingsContext context)
        {
            _context = context;
        }

        public IList<Setting> Setting { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Setting = await _context.Setting.ToListAsync();
        }
    }
}

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
    public class IndexModel : PageModel
    {
        private readonly BDFA.Data.DirectoryContext _context;

        public IndexModel(BDFA.Data.DirectoryContext context)
        {
            _context = context;
        }

        public IList<Profile> Profile { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Profile = await _context.Profiles.ToListAsync();
        }
    }
}

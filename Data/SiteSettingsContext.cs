using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BDFA.Models;

namespace BDFA.Data
{
    public class SiteSettingsContext : DbContext
    {
        public SiteSettingsContext (DbContextOptions<SiteSettingsContext> options)
            : base(options)
        {
        }

        public DbSet<BDFA.Models.Setting> Setting { get; set; } = default!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BDFA.Models;

namespace BDFA.Data
{
    public class SiteAdminContext : DbContext
    {
        public SiteAdminContext (DbContextOptions<SiteAdminContext> options)
            : base(options)
        {
        }

        public DbSet<BDFA.Models.Admin> Admin { get; set; } = default!;
    }
}

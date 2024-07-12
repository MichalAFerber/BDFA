using BDFA.Models;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Data
{
    public class DirectoryContext : DbContext
    {
        public DirectoryContext (DbContextOptions<DirectoryContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<ClickData> Clicks { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<ClickData>().ToTable("Clicks");
            modelBuilder.Entity<Profile>().ToTable("Profiles");
            modelBuilder.Entity<Setting>().ToTable("Settings");
        }
    }
}

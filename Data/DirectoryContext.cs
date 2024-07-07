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

        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<Profile>().ToTable("Profiles");
            modelBuilder.Entity<Setting>().ToTable("Settings");
        }
    }
}

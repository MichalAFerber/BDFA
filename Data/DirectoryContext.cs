using BDFA.Models;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Data
{
    public class DirectoryContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Admin> Admins { get; set; }
        public DbSet<ClickData> Clicks { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public DirectoryContext(DbContextOptions<DirectoryContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DirectoryContext");
                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<ClickData>().ToTable("Clicks");
            modelBuilder.Entity<Profile>().ToTable("Profiles");
            modelBuilder.Entity<Setting>().ToTable("Settings");
        }
    }
}

using BDFA.Models;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Data
{
    public class DirectoryContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // DbSet representing the Admins table
        public DbSet<Admin> Admins { get; set; }
        // DbSet representing the Clicks table
        public DbSet<ClickData> Clicks { get; set; }
        // DbSet representing the Profiles table
        public DbSet<Profile> Profiles { get; set; }
        // DbSet representing the Settings table
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
                // Get the connection string from the configuration and use SQLite
                var connectionString = _configuration.GetConnectionString("DirectoryContext");
                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the Admin entity to the Admins table
            modelBuilder.Entity<Admin>().ToTable("Admins");
            // Map the ClickData entity to the Clicks table
            modelBuilder.Entity<ClickData>().ToTable("Clicks");
            // Map the Profile entity to the Profiles table
            modelBuilder.Entity<Profile>().ToTable("Profiles");
            // Map the Setting entity to the Settings table
            modelBuilder.Entity<Setting>().ToTable("Settings");
        }
    }
}

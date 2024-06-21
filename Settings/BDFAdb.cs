using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace BDFA.Settings
{
    public class BDFAdbContext : DbContext
    {
        public BDFAdbContext(DbContextOptions<BDFAdbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        // Add DbSets for other entities
    }

    public class Admin
    {
        public int ID { get; set; }
        public int Active { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Expires { get; set; }
    }

    public class Profile
    {
        public int ID { get; set; }
        public string? Author { get; set; }
        public string? Email { get; set; }
        public string? Tagline { get; set; }
        public string? Tags { get; set; }
        public string? UrlStore { get; set; }
        public string? UrlNewsletter { get; set; }
        public string? UrlFBGroup { get; set; }
        public string? UrlFBPage { get; set; }
        public string? UrlIG { get; set; }
        public string? UrlTikTok { get; set; }
        public string? UrlThreads { get; set; }
        public string? UrlX { get; set; }
        public string? UrlOther { get; set; }
    }
}
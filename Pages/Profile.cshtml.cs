using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BDFA.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly BDFAdbContext _dbContext;

        public ProfileModel(BDFAdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string About { get; set; } = "This is where you can add a short bio about yourself. You can also add links to your store, newsletter, social media, and other platforms.";
        [BindProperty]
        public int ID { get; set; }
        [BindProperty]
        public byte[]? Image { get; set; }
        [BindProperty]
        public string? Author { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Tagline { get; set; }
        [BindProperty]
        public string? Tags { get; set; }
        [BindProperty]
        public string? UrlStore { get; set; }
        [BindProperty]
        public string? UrlNewsletter { get; set; }
        [BindProperty]
        public string? UrlFBGroup { get; set; }
        [BindProperty]
        public string? UrlFBPage { get; set; }
        [BindProperty]
        public string? UrlIG { get; set; }
        [BindProperty]
        public string? UrlTikTok { get; set; }
        [BindProperty]
        public string? UrlThreads { get; set; }
        [BindProperty]
        public string? UrlX { get; set; }
        [BindProperty]
        public string? UrlOther { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync();
            if (profile != null)
            {
                ID = profile.ID;
                Author = profile.Author;
                Email = profile.Email;
                Tagline = profile.Tagline;
                Tags = profile.Tags;
                UrlStore = profile.UrlStore;
                UrlNewsletter = profile.UrlNewsletter;
                UrlFBGroup = profile.UrlFBGroup;
                UrlFBPage = profile.UrlFBPage;
                UrlIG = profile.UrlIG;
                UrlTikTok = profile.UrlTikTok;
                UrlThreads = profile.UrlThreads;
                UrlX = profile.UrlX;
                UrlOther = profile.UrlOther;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync();
            if (profile != null)
            {
                profile.Author = Author;
                profile.Email = Email;
                profile.Tagline = Tagline;
                profile.Tags = Tags;
                profile.UrlStore = UrlStore;
                profile.UrlNewsletter = UrlNewsletter;
                profile.UrlFBGroup = UrlFBGroup;
                profile.UrlFBPage = UrlFBPage;
                profile.UrlIG = UrlIG;
                profile.UrlTikTok = UrlTikTok;
                profile.UrlThreads = UrlThreads;
                profile.UrlX = UrlX;
                profile.UrlOther = UrlOther;
            }
            else
            {
                profile = new Profile
                {
                    ID = ID,
                    Author = Author,
                    Email = Email,
                    Tagline = Tagline,
                    Tags = Tags,
                    UrlStore = UrlStore,
                    UrlNewsletter = UrlNewsletter,
                    UrlFBGroup = UrlFBGroup,
                    UrlFBPage = UrlFBPage,
                    UrlIG = UrlIG,
                    UrlTikTok = UrlTikTok,
                    UrlThreads = UrlThreads,
                    UrlX = UrlX,
                    UrlOther = UrlOther
                };
                _dbContext.Profiles.Add(profile);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToPage();
        }
    }
    public class BufferedSingleFileUploadDbModel : PageModel
    {
        [BindProperty]
        public required BufferedSingleFileUploadDb FileUpload { get; set; }
    }

    public class BufferedSingleFileUploadDb
    {
        [Required]
        [Display(Name = "Profile Image")]
        public required IFormFile Image { get; set; }
    }
    public class BDFAdbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "bdfa-master.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class Profile
    {
        public int ID { get; set; }
        public byte[]? Image { get; set; }
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

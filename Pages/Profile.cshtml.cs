using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BDFA.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public string About { get; set; } = "This is where you can add a short bio about yourself. You can also add links to your store, newsletter, social media, and other platforms.";
        [BindProperty]
        public int ID { get; set; }
        [BindProperty]
        public byte[] Image { get; set; }
        [BindProperty]
        public string Author { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Tagline { get; set; }
        [BindProperty]
        public string Tags { get; set; }
        [BindProperty]
        public string UrlStore { get; set; }
        [BindProperty]
        public string UrlNewsletter { get; set; }
        [BindProperty]
        public string UrlFBGroup { get; set; }
        [BindProperty]
        public string UrlFBPage { get; set; }
        [BindProperty]
        public string UrlIG { get; set; }
        [BindProperty]
        public string UrlTikTok { get; set; }
        [BindProperty]
        public string UrlThreads { get; set; }
        [BindProperty]
        public string UrlX { get; set; }
        [BindProperty]
        public string UrlOther { get; set; }
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
}
using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class Profile
    {
        [Key]
        public int ID { get; set; }
        public int Active { get; set; }
        public byte[] Image { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Tagline { get; set; }
        public string Tags { get; set; }
        public string UrlStore { get; set; }
        public string UrlNewsletter { get; set; }
        public string UrlFBGroup { get; set; }
        public string UrlFBPage { get; set; }
        public string UrlIG { get; set; }
        public string UrlTikTok { get; set; }
        public string UrlThreads { get; set; }
        public string UrlX { get; set; }
        public string UrlOther { get; set; }
        public string AuthToken { get; set; }
        public string Expires { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class Admin
    {
        [Key]
        public int ID { get; set; }
        public int Active { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string? AuthToken { get; set; }
        public DateTime? Expires { get; set; }
    }
}

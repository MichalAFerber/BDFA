using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class ClickData
    {
        [Key]
        public int ID { get; set; }
        public string ProfileId { get; set; }
        public string Link { get; set; }
        public DateTime ClickDateTime { get; set; }
    }
}

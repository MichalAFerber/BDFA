using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class Setting
    {
        [Key]
        public int ID { get; set; }

        public string DealImage1 { get; set; }
        public string DealURL1 { get; set; }

        public string DealImage2 { get; set; }
        public string DealURL2 { get; set; }

        public string DealImage3 { get; set; }
        public string DealURL3 { get; set; }

    }
}

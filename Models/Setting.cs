using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class Setting
    {
        [Key]
        public int ID { get; set; }
        public string Profile_About { get; set; }
    }
}

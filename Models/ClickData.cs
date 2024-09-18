using System.ComponentModel.DataAnnotations;

namespace BDFA.Models
{
    public class ClickData
    {
        [Key]
        public int ID { get; set; }
        public int ProfileId { get; set; }
        public string Link { get; set; }
        public DateTime ClickDateTime { get; set; }
    }

    public class ClickDataGroup
    {
        public string Link { get; set; }
        public int ClickCount { get; set; }
    }

    public class ClickDataFilter
    {
        public string ClickedLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

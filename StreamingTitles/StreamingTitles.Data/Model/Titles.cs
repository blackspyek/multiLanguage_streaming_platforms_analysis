using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingTitles.Data.Model
{
    [Table("Title")]
    public class Title
    {
        public int Id { get; set; }
        public ICollection<TitleCategory> TitleCategory { get; set; }
        public ICollection<TitlePlatform> TitlePlatform { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string TitleName { get; set; }

        public string Director { get; set; }
        public string Cast { get; set; }
        public string Country { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date_Added { get; set; }

        [Required]
        public int? Release_Year { get; set; }

    }


}

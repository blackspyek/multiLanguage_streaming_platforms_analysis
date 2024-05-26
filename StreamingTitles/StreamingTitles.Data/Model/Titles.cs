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
        public ICollection<TitleCountry> TitleCountry { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string TitleName { get; set; }


        [Required]
        public int? Release_Year { get; set; }

    }


}

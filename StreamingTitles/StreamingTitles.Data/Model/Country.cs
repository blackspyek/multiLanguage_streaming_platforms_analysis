using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingTitles.Data.Model
{
    [Table("Country")]
    [Index(nameof(Name), IsUnique = true)]
    public class Country
    {
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }

        public ICollection<TitleCountry> TitleCountry { get; set; }
    }
}

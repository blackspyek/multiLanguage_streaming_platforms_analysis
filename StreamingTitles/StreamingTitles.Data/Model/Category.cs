using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingTitles.Data.Model
{
    [Table("Category")]
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }

        public ICollection<TitleCategory> TitleCategory { get; set; }
    }
}

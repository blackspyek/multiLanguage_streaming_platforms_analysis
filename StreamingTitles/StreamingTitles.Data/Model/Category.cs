using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingTitles.Data.Model
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<TitleCategory> TitleCategory { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace StreamingTitles.Data.Model
{
    public class Platform
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Type must be at least 3 characters long.")]
        public string Name { get; set; }
        public ICollection<TitlePlatform> TitlePlatform { get; set; }

    }
}

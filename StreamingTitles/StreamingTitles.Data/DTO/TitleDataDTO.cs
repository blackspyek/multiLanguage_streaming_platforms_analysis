using System.Text.Json.Serialization;

namespace StreamingTitles.Data.DTO
{
    public class TitleDataDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TitleName { get; set; }

        public string Director { get; set; }
        public string Cast { get; set; }
        public string Country { get; set; }

        public DateTime? Date_Added { get; set; }

        public int? Release_Year { get; set; }
        [JsonPropertyName("Genre")]
        public ICollection<TitleCategoryDTO> TitleCategory { get; set; }
        [JsonPropertyName("Platform")]
        public ICollection<TitlePlatformDTO> TitlePlatform { get; set; }

    }
}

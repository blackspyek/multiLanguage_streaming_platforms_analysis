namespace StreamingTitles.Data.DTO
{
    public class FullTitleInfoDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TitleName { get; set; }

        public int? Release_Year { get; set; }

        public ICollection<TitleCategoryDTO> TitleCategory { get; set; }
        public ICollection<TitlePlatformDTO> TitlePlatform { get; set; }
        public ICollection<TitleCountryDTO> TitleCountry { get; set; }

    }
}

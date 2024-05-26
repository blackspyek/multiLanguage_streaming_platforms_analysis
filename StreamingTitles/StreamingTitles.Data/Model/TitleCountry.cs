namespace StreamingTitles.Data.Model
{
    public class TitleCountry
    {
        public int TitleId { get; set; }
        public int CountryId { get; set; }

        public Country Country { get; set; }
        public Title Title { get; set; }
    }
}

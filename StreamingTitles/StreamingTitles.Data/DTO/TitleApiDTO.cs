namespace StreamingTitles.Data.DTO
{
    public class TitleApiDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TitleName { get; set; }

        public string Country { get; set; }

        public int? Release_Year { get; set; }

        public string platformNames { get; set; }

        public string categoryNames { get; set; }


    }
}

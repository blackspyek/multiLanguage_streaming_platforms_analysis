namespace StreamingTitles.Data.DTO
{
    public class TitleDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TitleName { get; set; }

        public string Director { get; set; }
        public string Cast { get; set; }
        public string Country { get; set; }

        public DateTime? Date_Added { get; set; }

        public int? Release_Year { get; set; }


    }
}

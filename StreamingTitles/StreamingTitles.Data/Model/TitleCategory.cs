namespace StreamingTitles.Data.Model
{
    public class TitleCategory
    {
        public int TitleId { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public Title Title { get; set; }
    }
}

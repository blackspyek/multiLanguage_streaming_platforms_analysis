namespace StreamingTitles.Data.Model
{
    public class TitlePlatform
    {
        public int TitleId { get; set; }
        public int PlatformId { get; set; }

        public Platform Platform { get; set; }
        public Title Title { get; set; }
    }
}

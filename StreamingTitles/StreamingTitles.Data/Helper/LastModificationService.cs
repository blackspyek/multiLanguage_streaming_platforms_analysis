namespace StreamingTitles.Data.Helper
{
    public class LastModificationService : ILastModificationService
    {
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}


using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public interface ITitlesRepository
    {
        Task<Platform> GetPlatformByName(string name);
        Task<IEnumerable<Title>> GetTitlesAsync();
        Task<Title> GetTitleByIdAsync(int id);
        Task<IEnumerable<Title>> GetTitlesByYearAsync(int year);
        Task<IEnumerable<Title>> GetTitlesByReleaseYearRangeAsync(List<string> genreNames, List<string> platformNames, int startYear, int endYear);
        Task<bool> CreateTitle(int platformId, int categoryId, Title title);
        Task<bool> UpdateTitle(Title title);
        Task<bool> DeleteTitle(Title title);
        Task<bool> Save();

        // TODO: Repair an error when inserting same title in the same platform or other platform
        // TODO: Create InsertFromCSV method


    }
}
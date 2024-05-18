
using StreamingTitles.Data.Model;
using System.Xml.Linq;

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
        Task<bool> CreateTitleFromObject(List<TitlePlatform> platforms, List<TitleCategory> categories, List<Title> title);
        Task<bool> UpdateTitle(Title title);
        Task<bool> DeleteTitle(Title title);
        Task<bool> Save();

        Task<int> TitleExistsAsync(XDocument doc, string platformName);

        // TODO: Repair an error when inserting same title in the same platform or other platform
        // TODO: Create InsertFromCSV method


    }
}
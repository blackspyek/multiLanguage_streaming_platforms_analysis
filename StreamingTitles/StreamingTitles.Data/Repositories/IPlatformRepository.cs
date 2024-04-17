using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public interface IPlatformRepository
    {
        Task<Platform> GetPlatformById(int platformid);
        Task<Platform> GetPlatformByName(string platformname);

        Task<IEnumerable<Platform>> GetPlatformsAsync();
        Task<ICollection<Title>> GetTitlesByPlatformId(int platformid);
        Task<bool> CreatePlatform(Platform platform);
        Task<bool> UpdatePlatform(Platform platform);
        Task<bool> DeletePlatform(Platform platform);

        Task<bool> Save();
    }
}
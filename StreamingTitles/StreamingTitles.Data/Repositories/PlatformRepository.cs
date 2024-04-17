using Microsoft.EntityFrameworkCore;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly TitlesContext _ctx;
        public PlatformRepository(TitlesContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<IEnumerable<Platform>> GetPlatformsAsync()
        {
            var platforms = await _ctx.Platforms.AsNoTracking().ToListAsync();
            return platforms;
        }
        public async Task<Platform> GetPlatformById(int platformid)
        {
            Platform? platform = await _ctx.Platforms.AsNoTracking().FirstOrDefaultAsync(c => c.Id == platformid);
            return platform;
        }

        public Task<bool> CreatePlatform(Platform platform)
        {
            _ctx.Add(platform);
            return Save();
        }

        public Task<bool> Save()
        {
            var saved = _ctx.SaveChanges();
            return Task.FromResult(saved > 0);
        }

        public Task<bool> UpdatePlatform(Platform platform)
        {
            _ctx.Update(platform);
            return Save();
        }

        public Task<Platform> GetPlatformByName(string platformname)
        {
            Platform? platform = _ctx.Platforms.AsNoTracking().FirstOrDefault(c => c.Name == platformname);
            return Task.FromResult(platform);
        }

        public Task<bool> DeletePlatform(Platform platform)
        {
            _ctx.Remove(platform);
            return Save();
        }


        public async Task<ICollection<Title>> GetTitlesByPlatformId(int platformid)
        {
            return _ctx.TitlePlatform.AsNoTracking().Where(tc => tc.PlatformId == platformid).Select(tc => tc.Title).ToList();
        }
    }
}

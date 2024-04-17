using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories;
public class TitlesRepository : ITitlesRepository
{
    private readonly TitlesContext _ctx;
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _platformRepository;
    public TitlesRepository(TitlesContext ctx, IMapper mapper, IPlatformRepository platformRepository)
    {
        _ctx = ctx;
        _mapper = mapper;
        _platformRepository = platformRepository;
    }
    public async Task<IEnumerable<Title>> GetTitlesByReleaseYearRangeAsync(List<string> genreNames, List<string> platformNames, int startYear = 0, int endYear = 0)
    {
        if (endYear == 0)
        {
            endYear = DateTime.Now.Year;
        }
        else if (startYear == 0)
        {
            startYear = DateTime.UnixEpoch.Year;
        }

        var query = _ctx.Collection.AsNoTracking()
            .Include(t => t.TitleCategory)
            .ThenInclude(tc => tc.Category)
            .Include(t => t.TitlePlatform)
            .ThenInclude(tp => tp.Platform)
            .Where(t => t.Release_Year >= startYear && t.Release_Year <= endYear);

        if (genreNames != null && genreNames.Any())
        {
            query = query.Where(t => t.TitleCategory.Any(tc => genreNames.Contains(tc.Category.Name)));
        }
        if (platformNames != null && platformNames.Any())
        {
            query = query.Where(t => t.TitlePlatform.Any(tc => platformNames.Contains(tc.Platform.Name)));
        }
        var titles = await query.Distinct().ToListAsync();
        return titles;
    }
    public async Task<Platform> GetPlatformByName(string name)
    {
        try
        {
            Platform? platform = await _ctx.Platforms.FirstOrDefaultAsync(p => p.Name == name);
            return platform;
        }
        catch
        {
            Platform newPlatform = new Platform { Name = name };
            _ctx.Platforms.Add(newPlatform);
            await _ctx.SaveChangesAsync();
            return newPlatform;

        }
    }
    public async Task<IEnumerable<Title>> GetTitlesAsync()
    {
        var titles = await _ctx.Collection.AsNoTracking().ToListAsync();
        return titles;
    }
    public async Task<Title> GetTitleByIdAsync(int id)
    {
        var title = await _ctx.Collection.AsNoTracking()
            .Include(t => t.TitleCategory)
            .ThenInclude(tc => tc.Category)
            .Include(t => t.TitlePlatform)
            .ThenInclude(tp => tp.Platform)
            .FirstOrDefaultAsync(t => t.Id == id);
        return title;

    }

    public async Task<IEnumerable<Title>> GetTitlesByYearAsync(int year)
    {
        var titles = await _ctx.Collection.AsNoTracking()
            .Include(t => t.TitleCategory)
            .ThenInclude(tc => tc.Category)
            .Where(t => t.Release_Year == year)
            .ToListAsync();
        return titles;
    }

    public Task<bool> CreateTitle(int platformId, int categoryId, Title title)
    {
        var titlecategoryEntity = _ctx.Categories.FirstOrDefault(c => c.Id == categoryId);
        var platformEntity = _ctx.Platforms.FirstOrDefault(p => p.Id == platformId);
        var titlecategory = new TitleCategory
        {
            Category = titlecategoryEntity,
            Title = title
        };

        _ctx.Add(titlecategory);

        var titleplatform = new TitlePlatform
        {
            Platform = platformEntity,
            Title = title
        };
        _ctx.Add(titleplatform);
        _ctx.Add(title);
        return Save();
    }

    public Task<bool> Save()
    {
        var saved = _ctx.SaveChangesAsync().Result;
        return Task.FromResult(saved > 0);
    }

    public Task<bool> UpdateTitle(Title title)
    {
        _ctx.Update(title);
        return Save();
    }

    public async Task<bool> DeleteTitle(Title title)
    {
        var titlecategoryEntity = _ctx.TitleCategories.FirstOrDefault(tc => tc.TitleId == title.Id);
        if (titlecategoryEntity == null)
        {
            return false;
        }
        var titleplatformEntity = _ctx.TitlePlatform.FirstOrDefault(tp => tp.TitleId == title.Id);
        if (titleplatformEntity == null)
        {
            return false;
        }
        _ctx.TitleCategories.Remove(titlecategoryEntity);
        _ctx.TitlePlatform.Remove(titleplatformEntity);
        _ctx.Remove(title);
        return await Save();
    }
}

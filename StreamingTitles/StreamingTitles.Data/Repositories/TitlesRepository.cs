using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StreamingTitles.Data.Model;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace StreamingTitles.Data.Repositories;
public class TitlesRepository : ITitlesRepository
{
    private readonly TitlesContext _ctx;
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _platformRepository;
    private readonly IHubContext<ProgressHub> _hubContext;
    public TitlesRepository(TitlesContext ctx, IMapper mapper, IPlatformRepository platformRepository, IHubContext<ProgressHub> hubContext)
    {
        _ctx = ctx;
        _mapper = mapper;
        _platformRepository = platformRepository;
        _hubContext = hubContext = hubContext;
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

    public async Task<int> TitleExistsAsync(XDocument doc, string platformName)
    {
        var platform = await _platformRepository.GetPlatformByName(platformName);
        if (platform == null)
        {
            platform = new Platform { Name = platformName };
            _ctx.Platforms.Add(platform);
            await _ctx.SaveChangesAsync();
        }
        var allGenres = _ctx.Categories.AsNoTracking().ToList();

        var Genres = new ConcurrentDictionary<string, bool>();


        doc.Root.Elements("row").AsParallel().ForAll(row =>
        {
            var listedIn = row.Element("listed_in").Value;
            var genres = listedIn.Split(',');
            foreach (var genre in genres)
            {
                Genres.TryAdd(genre.Trim().ToLower(), true);
            }
        });

        foreach (var genre in Genres.Keys)
        {
            var temp = genre.Trim().ToLower();
            if (!allGenres.Any(n => n.Name == temp))
            {
                Category newCategory = new Category { Name = temp };
                _ctx.Categories.Add(newCategory);
            }
        }
        _ctx.SaveChanges();
        allGenres = _ctx.Categories.AsNoTracking().ToList();
        var totalRowCount = doc.Root.Elements("row").Count();
        var rowsProcessed = 0;
        /*
         * 
         * Transaction Default: read committed
         * 
         * 
         */
        doc.Root.Elements("row").AsParallel().ForAll(async row =>
            {
                var type = row.Element("type").Value;
                var title = row.Element("title").Value;
                var country = row.Element("country").Value;
                var listedIn = row.Element("listed_in").Value;
                var releaseYear = row.Element("release_year").Value;

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(releaseYear) || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(listedIn) || string.IsNullOrEmpty(type))
                {
                    return;
                }
                using (var titlesContext = new TitlesContext())
                {
                    try
                    {
                        var titleExists = titlesContext.Collection
                            .Include(t => t.TitlePlatform)
                            .Include(t => t.TitleCategory)
                            .AsNoTracking().FirstOrDefault(
                                t => t.TitleName == title && t.Release_Year == int.Parse(releaseYear));

                        var categoryNames = listedIn.Split(',');
                        string temp = "";
                        Category categoryEntity;

                        if (titleExists != null)
                        {
                            if (titleExists.TitlePlatform == null)
                                titleExists.TitlePlatform = new List<TitlePlatform>();

                            if (titleExists.TitleCategory == null)
                                titleExists.TitleCategory = new List<TitleCategory>();

                            if (titleExists.TitlePlatform.Any(tp => tp.PlatformId == platform.Id) == false)
                            {
                                TitlePlatform titlePlatform = new TitlePlatform
                                {
                                    PlatformId = platform.Id,
                                    TitleId = titleExists.Id
                                };
                                titleExists.TitlePlatform.Add(titlePlatform);
                            }


                            foreach (var categoryName in categoryNames)
                            {
                                temp = categoryName.Trim().ToLower();
                                categoryEntity = allGenres.FirstOrDefault(n => n.Name == temp);
                                if (titleExists.TitleCategory.Any(tm => tm.CategoryId == categoryEntity.Id) == false)
                                {

                                    TitleCategory titleCategory = new TitleCategory
                                    {
                                        CategoryId = categoryEntity.Id,
                                        TitleId = titleExists.Id
                                    };
                                    titleExists.TitleCategory.Add(titleCategory);

                                }
                                titlesContext.Collection.Update(titleExists);

                            }

                            titlesContext.SaveChanges();
                        }
                        else
                        {
                            Title newTitle = new Title
                            {
                                Type = type,
                                TitleName = title,
                                Country = country,
                                Release_Year = int.Parse(releaseYear)
                            };

                            TitlePlatform titlePlatform = new TitlePlatform
                            {
                                PlatformId = platform.Id,
                                TitleId = newTitle.Id
                            };
                            if (newTitle.TitlePlatform == null)
                                newTitle.TitlePlatform = new List<TitlePlatform>();

                            if (newTitle.TitleCategory == null)
                                newTitle.TitleCategory = new List<TitleCategory>();


                            foreach (var categoryName in categoryNames)
                            {
                                temp = categoryName.Trim().ToLower();
                                categoryEntity = allGenres.FirstOrDefault(n => n.Name == temp);
                                TitleCategory titleCategory = new TitleCategory
                                {
                                    CategoryId = categoryEntity.Id,
                                    TitleId = newTitle.Id
                                };
                                newTitle.TitleCategory.Add(titleCategory);
                            }

                            newTitle.TitlePlatform.Add(titlePlatform);
                            titlesContext.Collection.Add(newTitle);
                            titlesContext.SaveChanges();


                            Interlocked.Increment(ref rowsProcessed);

                            await _hubContext.Clients.All.SendAsync("ProgressUpdate", totalRowCount - rowsProcessed);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return;
                    }

                }

            });
        return 1;

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

    public async Task<bool> CreateTitleFromObject(List<TitlePlatform> platforms, List<TitleCategory> categories, List<Title> title)
    {
        _ctx.AddRange(categories);
        _ctx.AddRange(platforms);
        _ctx.AddRange(title);
        return await Save();

    }
    public async Task<bool> CreateTitleFromObjectApi(string platformNames, string categoryNames, Title title)
    {
        var platforms = platformNames.Split(",");
        var categories = categoryNames.Split(",");
        var platformEntities = new List<Platform>();
        var categoryEntities = new List<Category>();
        foreach (var platform in platforms)
        {
            var platformEntity = await _ctx.Platforms.FirstOrDefaultAsync(p => p.Name == platform);
            if (platformEntity != null)
            {
                platformEntities.Add(platformEntity);
            }
            else
            {
                var newPlatform = new Platform { Name = platform };
                _ctx.Platforms.Add(newPlatform);
                platformEntities.Add(newPlatform);
            }
        }
        foreach (var category in categories)
        {
            var categoryEntity = await _ctx.Categories.FirstOrDefaultAsync(c => c.Name == category);
            if (categoryEntity != null)
            {
                categoryEntities.Add(categoryEntity);
            }
            else
            {
                var newCategory = new Category { Name = category };
                _ctx.Categories.Add(newCategory);
                categoryEntities.Add(newCategory);
            }
        }
        await Save();
        var titlePlatforms = new List<TitlePlatform>();
        var titleCategories = new List<TitleCategory>();
        foreach (var platform in platformEntities)
        {
            var titlePlatform = new TitlePlatform
            {
                Platform = platform,
                Title = title
            };
            titlePlatforms.Add(titlePlatform);
        }
        foreach (var category in categoryEntities)
        {
            var titleCategory = new TitleCategory
            {
                Category = category,
                Title = title
            };
            titleCategories.Add(titleCategory);
        }
        _ctx.AddRange(titlePlatforms);
        _ctx.AddRange(titleCategories);
        _ctx.Add(title);
        return await Save();

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

using Microsoft.EntityFrameworkCore;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly TitlesContext _ctx;


        public CountryRepository(TitlesContext ctx)
        {
            _ctx = ctx;
        }
        /*
         Getters
         */

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var countries = await _ctx.Countries.AsNoTracking().ToListAsync();
            return countries;
        }

        public async Task<Country> GetCountryById(int countryid)
        {
            Country? country = await _ctx.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == countryid);
            return country;
        }

        public async Task<Country> GetCountryByName(string countryName)
        {
            Country? country = await _ctx.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Name == countryName);
            return country;
        }

        public async Task<ICollection<Title>> GetTitlesByCountryId(int countryid)
        {
            return _ctx.TitleCountry.AsNoTracking().Where(tc => tc.CountryId == countryid).Select(tc => tc.Title).ToList();
        }

        /*
         *         CUD
         *                 */
        public Task<bool> CreateCountry(Country country)
        {
            _ctx.Add(country);
            return Save();
        }
        public async Task<Dictionary<string, List<TitleDTO>>> GetAllCountriesTitles()
        {
            var titlesByCountry = await _ctx.TitleCountry
                .Include(tc => tc.Title)
                .Include(tc => tc.Country)
                .GroupBy(tc => tc.Country.Name)
                .AsNoTracking()
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(tc => new TitleDTO
                    {
                        Id = tc.Title.Id,
                        TitleName = tc.Title.TitleName,
                        Type = tc.Title.Type,
                        Release_Year = tc.Title.Release_Year,
                    }).ToList()
            );
            return titlesByCountry;
        }
        public async Task<bool> DeleteCountry(Country country)
        {
            _ctx.Countries.Remove(country);
            await _ctx.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateCountry(Country country)
        {
            _ctx.Countries.Update(country);
            await _ctx.SaveChangesAsync();
            return true;
        }


        // Save
        public Task<bool> Save()
        {
            var saved = _ctx.SaveChangesAsync();
            return saved.ContinueWith(task => task.Result > 0);
        }

        // Check if exists

        public Task<bool> CountryExists(int countryid)
        {
            return _ctx.Countries.AnyAsync(c => c.Id == countryid);
        }

        public async Task<bool> CountryTitleExists(int testCountryId, int testTitleId)
        {
            return await _ctx.TitleCountry.AsNoTracking().AnyAsync(tc => tc.CountryId == testCountryId && tc.TitleId == testTitleId);
        }
    }
}

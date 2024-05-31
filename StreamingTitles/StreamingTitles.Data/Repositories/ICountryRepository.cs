using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<Country> GetCountryById(int countryid);
        Task<Country> GetCountryByName(string countryName);

        Task<ICollection<Title>> GetTitlesByCountryId(int countryid);
        Task<bool> CreateCountry(Country country);
        Task<bool> UpdateCountry(Country country);
        Task<bool> DeleteCountry(Country country);
        Task<bool> Save();
        Task<bool> CountryExists(int countryid);
        Task<bool> CountryTitleExists(int testCountryId, int testTitleId);
        Task<Dictionary<string, List<TitleDTO>>> GetAllCountriesTitles();
    }
}
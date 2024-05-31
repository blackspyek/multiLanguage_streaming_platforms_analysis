using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

namespace StreamingTitles.Api.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<TitlesController> _logger;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, ILogger<TitlesController> logger, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Country>), 200)]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = _mapper.Map<List<CountryDTO>>(await _countryRepository.GetCountriesAsync());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(countries);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new
                {
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Country), 200)]
        public async Task<IActionResult> GetCountryById(int id)
        {
            try
            {
                var country = _mapper.Map<CountryDTO>(await _countryRepository.GetCountryById(id));
                if (country == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = "Title not found"
                    });

                }
                return Ok(country);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(new
                {
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet("all/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Title>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTitleByCountryId(int id)
        {
            if (!_countryRepository.CountryExists(id).Result)
            {
                ModelState.AddModelError("", "Country does not exist");
                return StatusCode(400, ModelState);
            }
            var titles = _mapper.Map<List<TitleDTO>>(
                await _countryRepository.GetTitlesByCountryId(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(titles);
        }

        [HttpGet("all/movies")]
        [ProducesResponseType(typeof(Dictionary<string, List<TitleDTO>>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCountryWithTitles()
        {
            try
            {
                var titlesByCountry = await _countryRepository.GetAllCountriesTitles();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(titlesByCountry);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new
                {
                    StatusCode = 500,
                    message = ex.Message
                });
            }

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDTO countryCreate)
        {
            if (countryCreate == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Country is null"
                });
            }
            var country = _countryRepository.GetCountriesAsync()
                .Result.FirstOrDefault(c => c.Name == countryCreate.Name);
            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryMap = _mapper.Map<Country>(countryCreate);
            if (!await _countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the Country {countryCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created!");

        }
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCountry(int countryId, [FromBody] CountryDTO updatedCountry)
        {
            if (updatedCountry == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Country is null"
                });
            }
            if (countryId != updatedCountry.Id)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Country Id mismatch"
                });
            }
            var country = _countryRepository.GetCountryById(countryId).Result;
            var countryExists = _countryRepository.GetCountryByName(updatedCountry.Name).Result;

            if (country == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Country not found"
                });
            }
            if (countryExists != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryMap = _mapper.Map<Country>(updatedCountry);
            if (!await _countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the country {updatedCountry.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            var country = _countryRepository.GetCountryById(countryId).Result;
            if (country == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Country not found"
                });
            }
            var titles = _countryRepository.GetTitlesByCountryId(countryId).Result;
            if (titles.Count > 0)
            {
                ModelState.AddModelError("", "Country has associated titles. Please remove them first");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _countryRepository.DeleteCountry(country))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the Country {country.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}

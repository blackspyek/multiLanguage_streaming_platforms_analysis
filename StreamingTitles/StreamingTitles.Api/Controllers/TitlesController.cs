﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

namespace StreamingTitles.Api.Controllers
{
    [Route("api/titles")]
    [ApiController]
    public class TitlesController : ControllerBase
    {
        private readonly ITitlesRepository _titlesRepo;
        private readonly IPlatformRepository _platformRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<TitlesController> _logger;
        private readonly IMapper _mapper;
        public TitlesController(ITitlesRepository titlesRepo, ILogger<TitlesController> logger, IMapper mapper,
                IPlatformRepository platformRepository,
                ICategoryRepository categoriesRepository)
        {
            _titlesRepo = titlesRepo;
            _logger = logger;
            _mapper = mapper;
            _platformRepository = platformRepository;
            _categoryRepository = categoriesRepository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Title), 200)]
        public async Task<IActionResult> GetTitles(int id)
        {
            try
            {
                var title = _mapper.Map<TitleDataDTO>(await _titlesRepo.GetTitleByIdAsync(id));
                if (title == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = "Title not found"
                    });

                }
                return Ok(title);

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

        [HttpGet("byYear/{year}")]
        [ProducesResponseType(typeof(IEnumerable<Title>), 200)]
        public async Task<IActionResult> GetTitlesByYear(int year)
        {
            try
            {
                var titles = _mapper.Map<List<TitleDTO>>(await _titlesRepo.GetTitlesByYearAsync(year));

                if (titles.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = "Titles not found"
                    });

                }
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(titles);

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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Title>), 200)]
        public async Task<IActionResult> GetTitlesByReleaseYearRange([FromQuery] int startYear,
                                                                     [FromQuery] int endYear,
                                                                     [FromQuery] string? genreNames,
                                                                     [FromQuery] string? platformNames)
        {
            try
            {
                List<string> genres = !string.IsNullOrEmpty(genreNames) ? genreNames.Split(',').ToList() : new List<string>();
                List<string> platforms = !string.IsNullOrEmpty(platformNames) ? platformNames.Split(',').ToList() : new List<string>();

                if (startYear > endYear && startYear != 0 && endYear != 0)
                    return BadRequest("Start year must be before or equal to end year");

                var titles = await _titlesRepo.GetTitlesByReleaseYearRangeAsync(genres, platforms, startYear, endYear);
                var titlesDTO = _mapper.Map<List<TitleDTO>>(titles);

                return Ok(titlesDTO);
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
        public async Task<IActionResult> CreateTitle([FromQuery] int platformId, [FromQuery] int categoryId, [FromBody] TitleDTO titleCreate)
        {
            if (titleCreate == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Title is null"
                });
            }
            var title = _titlesRepo.GetTitlesAsync()
                .Result.FirstOrDefault(c => c.TitleName == titleCreate.TitleName.TrimEnd()
                                        && c.Director == titleCreate.Director.TrimEnd()
                                        && c.Release_Year == titleCreate.Release_Year);
            if (title != null)
            {
                ModelState.AddModelError("", "Title already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var titleMap = _mapper.Map<Title>(titleCreate);

            if (!await _titlesRepo.CreateTitle(platformId, categoryId, titleMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the platform {titleCreate.TitleName}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created!");

        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateTitle(
                                [FromBody] TitleDTO updatedTitle)
        {
            if (updatedTitle == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Title is null"
                });
            }
            var title = _titlesRepo.GetTitleByIdAsync(updatedTitle.Id).Result;
            if (title == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Title not found"
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var titleMap = _mapper.Map<Title>(updatedTitle);
            if (!await _titlesRepo.UpdateTitle(titleMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the title {updatedTitle.TitleName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{titleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int titleId)
        {
            var title = _titlesRepo.GetTitleByIdAsync(titleId).Result;
            if (title == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Title not found"
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _titlesRepo.DeleteTitle(title))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the category {title.TitleName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

namespace StreamingTitles.Api.Controllers
{
    [Route("api/platform")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly ILogger<TitlesController> _logger;
        private readonly IMapper _mapper;

        public PlatformController(IPlatformRepository platformRepository, ILogger<TitlesController> logger, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Platform>), 200)]
        public async Task<IActionResult> GetPlatforms()
        {
            try
            {
                var platforms = _mapper.Map<List<PlatformDTO>>(await _platformRepository.GetPlatformsAsync());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(platforms);

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
        [ProducesResponseType(typeof(Platform), 200)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var title = _mapper.Map<PlatformDTO>(await _platformRepository.GetPlatformById(id));
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
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlatform([FromBody] PlatformDTO platformCreate)
        {
            if (platformCreate == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Platform is null"
                });
            }
            var platform = _platformRepository.GetPlatformsAsync()
                .Result.FirstOrDefault(c => c.Name == platformCreate.Name);
            if (platform != null)
            {
                ModelState.AddModelError("", "Platform already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var platformMap = _mapper.Map<Platform>(platformCreate);
            if (!await _platformRepository.CreatePlatform(platformMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the platform {platformCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created!");

        }
        [HttpPut("{platformId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCategory(int platformId, [FromBody] PlatformDTO updatedPlatform)
        {
            if (updatedPlatform == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Platform is null"
                });
            }
            if (platformId != updatedPlatform.Id)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Platform Id mismatch"
                });
            }
            var platform = _platformRepository.GetPlatformById(platformId).Result;
            var platformExists = _platformRepository.GetPlatformByName(updatedPlatform.Name).Result;

            if (platform == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Platform not found"
                });
            }
            if (platformExists != null)
            {
                ModelState.AddModelError("", "Platform already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var platformMap = _mapper.Map<Platform>(updatedPlatform);
            if (!await _platformRepository.UpdatePlatform(platformMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the category {updatedPlatform.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{platformId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int platformId)
        {
            var platform = _platformRepository.GetPlatformById(platformId).Result;
            if (platform == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Platform not found"
                });
            }
            var titles = _platformRepository.GetTitlesByPlatformId(platformId).Result;
            if (titles.Count > 0)
            {
                ModelState.AddModelError("", "Category has associated titles. Please remove them first");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _platformRepository.DeletePlatform(platform))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the category {platform.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

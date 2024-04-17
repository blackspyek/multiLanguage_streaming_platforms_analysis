using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

namespace StreamingTitles.Api.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<TitlesController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, ILogger<TitlesController> logger, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), 200)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = _mapper.Map<List<CategoryDTO>>(await _categoryRepository.GetCategoriesAsync());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(categories);

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
        [ProducesResponseType(typeof(Category), 200)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var title = _mapper.Map<CategoryDTO>(await _categoryRepository.GetCategoryById(id));
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
        [HttpGet("all/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Title>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTitleByCategoryId(int id)
        {
            if (!_categoryRepository.CategoryExists(id).Result)
            {
                ModelState.AddModelError("", "Category does not exist");
                return StatusCode(400, ModelState);
            }
            var titles = _mapper.Map<List<TitleDTO>>(
                await _categoryRepository.GetTitlesByCategoryId(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(titles);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryCreate)
        {
            if (categoryCreate == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Category is null"
                });
            }
            var category = _categoryRepository.GetCategoriesAsync()
                .Result.FirstOrDefault(c => c.Name == categoryCreate.Name);
            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryCreate);
            if (!await _categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the category {categoryCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created!");

        }
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDTO updatedCategory)
        {
            if (updatedCategory == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Category is null"
                });
            }
            if (categoryId != updatedCategory.Id)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    message = "Category Id mismatch"
                });
            }
            var category = _categoryRepository.GetCategoryById(categoryId).Result;
            var categoryExists = _categoryRepository.GetCategoryByName(updatedCategory.Name).Result;

            if (category == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Category not found"
                });
            }
            if (categoryExists != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(updatedCategory);
            if (!await _categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the category {updatedCategory.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId).Result;
            if (category == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    message = "Category not found"
                });
            }
            var titles = _categoryRepository.GetTitlesByCategoryId(categoryId).Result;
            if (titles.Count > 0)
            {
                ModelState.AddModelError("", "Category has associated titles. Please remove them first");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the category {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}

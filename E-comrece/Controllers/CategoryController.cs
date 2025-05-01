using E_comerce.Services;
using E_commerce.Dtos;
using E_comrece.DTO;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(CategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = $"Category with ID {id} not found." });

            return Ok(category);
        }

        // New endpoint to get all categories for a given storeId
        [HttpGet("store/{storeId:int}")]
        public async Task<IActionResult> GetCategoriesByStore(int storeId)
        {
            var storeExists = await _categoryService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            var categories = await _categoryService.GetCategoriesByStoreIdAsync(storeId);
            return Ok(categories); // Always return 200 with the list (even if it's empty)
        }


        // Endpoint to get category by storeId and categoryId
        [HttpGet("{storeId:int}/{categoryId:int}")]
        public async Task<IActionResult> GetCategoryByStoreAndId(int storeId, int categoryId)
        {
            // First, check if the store exists
            var storeExists = await _categoryService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            // Now, fetch the category by both storeId and categoryId
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null || category.StoreId != storeId)
            {
                return NotFound(new { message = $"Category with ID {categoryId} not found for store ID {storeId}." });
            }

            return Ok(category);
        }

        [HttpPost("{storeId:int}")]
        public async Task<IActionResult> CreateCategory(int storeId, [FromBody] CategoryCreateDto categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    return BadRequest(new { message = "Invalid category data." });

                categoryDto.StoreId = storeId;
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { storeId = storeId, id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category for storeId {StoreId}", storeId);
                return StatusCode(500, new { message = "An error occurred while creating the category." });
            }
        }

        [HttpPut("{storeId:int}/{id:int}")]
        public async Task<IActionResult> UpdateCategory(int storeId, int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(storeId, id, categoryDto);
            if (updatedCategory == null)
                return NotFound(new { message = $"Category with ID {id} not found for store ID {storeId}." });

            return Ok(updatedCategory);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid category ID." });
            }

            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
                return NotFound(new { message = $"Category with ID {id} not found." });

            return NoContent();
        }
    }
}

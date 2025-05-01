using E_comerce.Services;
using E_comerce.DTO;
using E_comerce.Models;
using Microsoft.AspNetCore.Mvc;


namespace E_comerce.Controllers
{
    [ApiController]
    [Route("api/sizes")]
    public class SizeController : ControllerBase
    {
        private readonly SizeService _sizeService;

        public SizeController(SizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // ✅ Get all sizes
        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _sizeService.GetAllSizesAsync();
            return Ok(sizes);
        }

        // ✅ Get sizes by StoreId (with validation)
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetSizesByStore(int storeId)
        {
            if (storeId <= 0)
            {
                return BadRequest(new { message = "Invalid store ID." });
            }

            var storeExists = await _sizeService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            var sizes = await _sizeService.GetSizesByStoreIdAsync(storeId);
            return Ok(sizes);
        }

        // ✅ Get single size by ID (with validation)
        [HttpGet("{id:int}")]  // Ensure ID is an integer
        public async Task<IActionResult> GetSizeById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid size ID." });
            }

            var size = await _sizeService.GetSizeByIdAsync(id);
            if (size == null)
                return NotFound(new { message = $"Size with ID {id} not found." });

            return Ok(size);
        }

        // ✅ Create a new size (ensure StoreId exists)
        [HttpPost("{storeId}/add-size")]
        public async Task<IActionResult> CreateSize(int storeId, [FromBody] SizeDto sizeDto)
        {
            if (sizeDto == null)
            {
                return BadRequest(new { message = "Size data is required." });
            }

            if (string.IsNullOrWhiteSpace(sizeDto.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            // Validate Store ID
            var storeExists = await _sizeService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            sizeDto.StoreId = storeId; // Ensure correct store ID is assigned

            var newSize = await _sizeService.CreateSizeAsync(sizeDto);
            return CreatedAtAction(nameof(GetSizeById), new { id = newSize.Id }, newSize);
        }

        // ✅ Update a size
        [HttpPut("{sizeId:int}")]
        public async Task<IActionResult> UpdateSize(int sizeId, [FromBody] SizeDto sizeDto)
        {
            if (sizeDto == null)
            {
                return BadRequest(new { message = "Size data is required." });
            }

            var sizeExists = await _sizeService.GetSizeByIdAsync(sizeId);
            if (sizeExists == null)
            {
                return NotFound(new { message = $"Size with ID {sizeId} not found." });
            }

            var updatedSize = await _sizeService.UpdateSizeAsync(sizeId, sizeDto);
            return Ok(updatedSize);
        }

        // ✅ Delete a size (with validation)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid size ID." });
            }

            var success = await _sizeService.DeleteSizeAsync(id);
            if (!success)
                return NotFound(new { message = $"Size with ID {id} not found." });

            return NoContent(); // Return 204 No Content for successful deletion
        }
    }
}

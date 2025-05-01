using E_comerce.Services;
using E_comerce.DTO;
using E_comerce.Models;
using Microsoft.AspNetCore.Mvc;
using E_comrece.DTO;


namespace E_comerce.Controllers
{
    [ApiController]
    [Route("api/colors")]
    public class ColorController : ControllerBase
    {
        private readonly ColorService _colorService;

        public ColorController(ColorService colorService)
        {
            _colorService = colorService;
        }

        // ✅ Get all colors
        [HttpGet]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _colorService.GetAllColorsAsync();
            return Ok(colors);
        }

        // ✅ Get colors by StoreId (with validation)
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetColorsByStore(int storeId)
        {
            if (storeId <= 0)
            {
                return BadRequest(new { message = "Invalid store ID." });
            }

            var storeExists = await _colorService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            var colors = await _colorService.GetColorsByStoreIdAsync(storeId);
            return Ok(colors);
        }

        // ✅ Get single color by ID (with validation)
        [HttpGet("{id:int}")]  // Ensure ID is an integer
        public async Task<IActionResult> GetColorById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid color ID." });
            }

            var color = await _colorService.GetColorByIdAsync(id);
            if (color == null)
                return NotFound(new { message = $"Color with ID {id} not found." });

            return Ok(color);
        }

        // ✅ Create a new color (ensure StoreId exists)
        [HttpPost("{storeId}/add-color")]
        public async Task<IActionResult> CreateColor(int storeId, [FromBody] ColorDto colorDto)
        {
            if (colorDto == null)
            {
                return BadRequest(new { message = "Color data is required." });
            }

            if (string.IsNullOrWhiteSpace(colorDto.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            // Validate Store ID
            var storeExists = await _colorService.DoesStoreExistAsync(storeId);
            if (!storeExists)
            {
                return NotFound(new { message = $"Store with ID {storeId} not found." });
            }

            colorDto.StoreId = storeId; // Ensure correct store ID is assigned

            var newColor = await _colorService.CreateColorAsync(colorDto);
            return CreatedAtAction(nameof(GetColorById), new { id = newColor.Id }, newColor);
        }

        // ✅ Update a color
        [HttpPut("{colorId:int}")]
        public async Task<IActionResult> UpdateColor(int colorId, [FromBody] ColorDto colorDto)
        {
            if (colorDto == null)
            {
                return BadRequest(new { message = "Color data is required." });
            }

            var colorExists = await _colorService.GetColorByIdAsync(colorId);
            if (colorExists == null)
            {
                return NotFound(new { message = $"Color with ID {colorId} not found." });
            }

            var updatedColor = await _colorService.UpdateColorAsync(colorId, colorDto);
            return Ok(updatedColor);
        }

        // ✅ Delete a color (with validation)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid color ID." });
            }

            var success = await _colorService.DeleteColorAsync(id);
            if (!success)
                return NotFound(new { message = $"Color with ID {id} not found." });

            return NoContent(); // Return 204 No Content for successful deletion
        }
    }
}

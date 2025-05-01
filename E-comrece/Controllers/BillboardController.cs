using E_comerce.Services;
using E_comerce.DTO;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/billboards")]
public class BillboardController : ControllerBase
{
    private readonly BillboardService _billboardService;

    public BillboardController(BillboardService billboardService)
    {
        _billboardService = billboardService;
    }

    // ✅ Get all billboards
    [HttpGet]
    public async Task<IActionResult> GetBillboards()
    {
        var billboards = await _billboardService.GetAllBillboardsAsync();
        return Ok(billboards);
    }

    // ✅ Get billboards by StoreId (with validation)
    [HttpGet("store/{storeId}")]
    public async Task<IActionResult> GetBillboardsByStore(int storeId)
    {
        if (storeId <= 0)
        {
            return BadRequest(new { message = "Invalid store ID." });
        }

        var storeExists = await _billboardService.DoesStoreExistAsync(storeId);
        if (!storeExists)
        {
            return NotFound(new { message = $"Store with ID {storeId} not found." });
        }

        var billboards = await _billboardService.GetBillboardsByStoreIdAsync(storeId);
        return Ok(billboards);
    }

    // ✅ Get single billboard by ID (with validation)
    [HttpGet("{id:int}")]  // Ensure ID is an integer
    public async Task<IActionResult> GetBillboardById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid billboard ID." });
        }

        var billboard = await _billboardService.GetBillboardByIdAsync(id);
        if (billboard == null)
            return NotFound(new { message = $"Billboard with ID {id} not found." });

        return Ok(billboard);
    }
    // ✅ Create a new billboard (ensure StoreId exists)
    [HttpPost("{storeId}/add-billboard")]
    public async Task<IActionResult> CreateBillboard(int storeId, [FromBody] BillboardDto billboardDto)
    {
        if (billboardDto == null)
        {
            return BadRequest(new { message = "Billboard data is required." });
        }

        if (string.IsNullOrWhiteSpace(billboardDto.Label) || string.IsNullOrWhiteSpace(billboardDto.ImageUrl))
        {
            return BadRequest(new { message = "Both Label and ImageUrl are required." });
        }

        // Validate Store ID
        var storeExists = await _billboardService.DoesStoreExistAsync(storeId);
        if (!storeExists)
        {
            return NotFound(new { message = $"Store with ID {storeId} not found." });
        }

        billboardDto.StoreId = storeId; // Ensure correct store ID is assigned

        var newBillboard = await _billboardService.CreateBillboardAsync(billboardDto);
        return CreatedAtAction(nameof(GetBillboardById), new { id = newBillboard.Id }, newBillboard);
    }

    [HttpPut("{billboardId:int}")]
    public async Task<IActionResult> UpdateBillboard( int billboardId, [FromBody] BillboardDto billboardDto)
    {
        if (billboardDto == null)
        {
            return BadRequest(new { message = "Billboard data is required." });
        }



        var billboardExists = await _billboardService.GetBillboardByIdAsync(billboardId);
        if (billboardExists == null)
        {
            return NotFound(new { message = $"Billboard with ID {billboardId} not found." });
        }


        var updatedBillboard = await _billboardService.UpdateBillboardAsync(billboardId, billboardDto);
        return Ok(updatedBillboard);
    }
    // ✅ Delete a billboard (with validation)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBillboard(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid billboard ID." });
        }

        var success = await _billboardService.DeleteBillboardAsync(id);
        if (!success)
            return NotFound(new { message = $"Billboard with ID {id} not found." });

        return NoContent(); // Return 204 No Content for successful deletion
    }
}

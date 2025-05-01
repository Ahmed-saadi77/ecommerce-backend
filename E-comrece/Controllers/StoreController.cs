using E_comerce.Models;
using E_comerce.Services;
using E_commerce.DTO;
using E_comrece.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/stores")]
[Authorize] // Require JWT authentication
public class StoreController : ControllerBase
{
    private readonly StoreService _storeService;

    public StoreController(StoreService storeService)
    {
        _storeService = storeService;
    }

    // Get all stores for the authenticated user
    [HttpGet]
    public async Task<IActionResult> GetStores()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from token
        var stores = await _storeService.GetStoresByUserAsync(userId);
        return Ok(stores);
    }

    // Get a specific store by ID for the authenticated user
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStoreById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var store = await _storeService.GetStoreByIdAsync(id);

        if (store == null || store.UserId != userId)
            return NotFound(new { message = "Store not found or access denied." });

        return Ok(store);
    }

    // Get the last store created by the authenticated user
    [HttpGet("last")]
    public async Task<IActionResult> GetLastStore()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var lastStore = await _storeService.GetLastStoreByUserAsync(userId); // Ensure this is user-specific

        return Ok(new { lastStoreId = lastStore?.Id });
    }

    // Create a new store for the authenticated user
    [HttpPost]
    public async Task<ActionResult<CreateStoreDto>> CreateStore([FromBody] CreateStoreDto store)
    {
        if (store == null || string.IsNullOrWhiteSpace(store.Name))
            return BadRequest(new { message = "Store name is required." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // From JWT
        store.UserId = userId; // Assign authenticated user ID

        var createdStore = await _storeService.CreateStoreAsync(store);

        if (createdStore == null || createdStore.Id == 0)
            return BadRequest(new { message = "Failed to create store." });

        return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, new
        {
            id = createdStore.Id,
            name = createdStore.Name,
            userId = createdStore.UserId,
            createdAt = createdStore.CreatedAt,
            updatedAt = createdStore.UpdatedAt
        });
    }

    // Update a store by ID for the authenticated user
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreDto storeUpdate)
    {
        if (storeUpdate == null || string.IsNullOrWhiteSpace(storeUpdate.Name))
            return BadRequest(new { message = "Store name is required." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var store = await _storeService.GetStoreByIdAsync(id);

        if (store == null || store.UserId != userId)
            return NotFound(new { message = "Store not found or access denied." });

        var success = await _storeService.UpdateStoreNameAsync(id, storeUpdate.Name);

        return success ? NoContent() : BadRequest(new { message = "Failed to update store." });
    }

    // Delete a store by ID for the authenticated user
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var store = await _storeService.GetStoreByIdAsync(id);

        if (store == null || store.UserId != userId)
            return NotFound(new { message = "Store not found or access denied." });

        var success = await _storeService.DeleteStoreAsync(id);

        if (!success)
            return BadRequest(new { message = "Failed to delete store." });

        var lastStore = await _storeService.GetLastStoreByUserAsync(userId); // Filtered by user

        return Ok(new { lastStoreId = lastStore?.Id });
    }

    // Get stores for a specific user by userId (updated endpoint to avoid ambiguity)
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetStoresByUser(string userId)
    {
        var stores = await _storeService.GetStoresByUserAsync(userId);
        return Ok(stores);
    }
}

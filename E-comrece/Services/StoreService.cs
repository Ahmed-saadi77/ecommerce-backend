using E_comerce.Data;
using E_comerce.Models;
using E_comrece.DTO;
using Microsoft.EntityFrameworkCore;

namespace E_comerce.Services
{
    public class StoreService
    {
        private readonly AppDbContext _context;

        public StoreService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Fetch all stores for a specific user
        public async Task<List<Store>> GetStoresByUserAsync(string userId)
        {
            try
            {
                return await _context.Stores
                    .Where(store => store.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching the stores for the user.", ex);
            }
        }

        // ✅ Fetch a store by its ID
        public async Task<Store?> GetStoreByIdAsync(int id)
        {
            try
            {
                return await _context.Stores.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while fetching the store with ID {id}.", ex);
            }
        }

        // ✅ Create a new store for the authenticated user
        public async Task<CreateStoreDto> CreateStoreAsync(CreateStoreDto storeDto)
        {
            try
            {
                var store = new Store
                {
                    Name = storeDto.Name,
                    UserId = storeDto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                return new CreateStoreDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    UserId = store.UserId,
                    CreatedAt = store.CreatedAt,
                    UpdatedAt = store.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the store.", ex);
            }
        }

        // ✅ Update the store name
        public async Task<bool> UpdateStoreNameAsync(int id, string newName)
        {
            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null) return false;

                store.Name = newName;
                store.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while updating the store with ID {id}.", ex);
            }
        }

        // ✅ Delete a store by ID
        public async Task<bool> DeleteStoreAsync(int id)
        {
            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null) return false;

                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the store with ID {id}.", ex);
            }
        }

        // ✅ Fetch the last created store for a specific user
        public async Task<Store?> GetLastStoreByUserAsync(string userId)
        {
            try
            {
                return await _context.Stores
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching the last store for the user.", ex);
            }
        }
    }
}

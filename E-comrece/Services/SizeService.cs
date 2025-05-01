using E_comerce.Models;
using E_comerce.DTO;
using Microsoft.EntityFrameworkCore;
using E_comerce.Data;

namespace E_comerce.Services
{
    public class SizeService
    {
        private readonly AppDbContext _context;

        public SizeService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all sizes
        public async Task<List<SizeDto>> GetAllSizesAsync()
        {
            // Mapping the Size entities to SizeDto objects
            return await _context.Sizes
                .Select(s => new SizeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StoreId = s.StoreId,
                    Value = s.Value,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        // ✅ Get sizes by StoreId
        public async Task<List<SizeDto>> GetSizesByStoreIdAsync(int storeId)
        {
            // Mapping the Size entities to SizeDto objects filtered by StoreId
            return await _context.Sizes
                .Where(s => s.StoreId == storeId)
                .Select(s => new SizeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StoreId = s.StoreId,
                    Value = s.Value,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        // ✅ Get a single size by ID
        public async Task<SizeDto?> GetSizeByIdAsync(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null) return null;

            return new SizeDto
            {
                Id = size.Id,
                Name = size.Name,
                StoreId = size.StoreId,
                Value = size.Value,
                CreatedAt = size.CreatedAt,
                UpdatedAt = size.UpdatedAt
            };
        }

        // ✅ Check if the store exists
        public async Task<bool> DoesStoreExistAsync(int storeId)
        {
            return await _context.Stores.AnyAsync(s => s.Id == storeId);
        }

        // ✅ Create a new size
        public async Task<SizeDto> CreateSizeAsync(SizeDto sizeDto)
        {
            // Creating a new Size entity from the SizeDto
            var size = new Size
            {
                Name = sizeDto.Name,
                StoreId = sizeDto.StoreId,
                Value = sizeDto.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();

            // Returning the newly created size as SizeDto
            return new SizeDto
            {
                Id = size.Id,
                Name = size.Name,
                StoreId = size.StoreId,
                Value = size.Value,
                CreatedAt = size.CreatedAt,
                UpdatedAt = size.UpdatedAt
            };
        }

        // ✅ Update an existing size
        public async Task<SizeDto> UpdateSizeAsync(int sizeId, SizeDto sizeDto)
        {
            var size = await _context.Sizes.FindAsync(sizeId);
            if (size == null) throw new KeyNotFoundException("Size not found.");

            size.Name = sizeDto.Name;
            size.Value = sizeDto.Value;
            size.UpdatedAt = DateTime.UtcNow;

            _context.Sizes.Update(size);
            await _context.SaveChangesAsync();

            return new SizeDto
            {
                Id = size.Id,
                Name = size.Name,
                StoreId = size.StoreId,
                Value = size.Value,
                CreatedAt = size.CreatedAt,
                UpdatedAt = size.UpdatedAt
            };
        }

        // ✅ Delete a size
        public async Task<bool> DeleteSizeAsync(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null) return false;

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

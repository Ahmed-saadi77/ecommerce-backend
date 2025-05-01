using E_comerce.Models;
using Microsoft.EntityFrameworkCore;
using E_comerce.Data;
using E_comrece.DTO;


namespace E_comerce.Services
{
    public class ColorService
    {
        private readonly AppDbContext _context;

        public ColorService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all colors
        public async Task<List<ColorDto>> GetAllColorsAsync()
        {
            return await _context.Colors
                .Select(c => new ColorDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StoreId = c.StoreId,
                    Value = c.Value,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        // ✅ Get colors by StoreId
        public async Task<List<ColorDto>> GetColorsByStoreIdAsync(int storeId)
        {
            return await _context.Colors
                .Where(c => c.StoreId == storeId)
                .Select(c => new ColorDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StoreId = c.StoreId,
                    Value = c.Value,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        // ✅ Get a single color by ID
        public async Task<ColorDto?> GetColorByIdAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return null;

            return new ColorDto
            {
                Id = color.Id,
                Name = color.Name,
                StoreId = color.StoreId,
                Value = color.Value,
                CreatedAt = color.CreatedAt,
                UpdatedAt = color.UpdatedAt
            };
        }

        // ✅ Check if the store exists
        public async Task<bool> DoesStoreExistAsync(int storeId)
        {
            return await _context.Stores.AnyAsync(s => s.Id == storeId);
        }

        // ✅ Create a new color
        public async Task<ColorDto> CreateColorAsync(ColorDto colorDto)
        {
            var color = new Color
            {
                Name = colorDto.Name,
                StoreId = colorDto.StoreId,
                Value = colorDto.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return new ColorDto
            {
                Id = color.Id,
                Name = color.Name,
                StoreId = color.StoreId,
                Value = color.Value,
                CreatedAt = color.CreatedAt,
                UpdatedAt = color.UpdatedAt
            };
        }

        // ✅ Update an existing color
        public async Task<ColorDto> UpdateColorAsync(int colorId, ColorDto colorDto)
        {
            var color = await _context.Colors.FindAsync(colorId);
            if (color == null) throw new KeyNotFoundException("Color not found.");

            color.Name = colorDto.Name;
            color.Value = colorDto.Value;
            color.UpdatedAt = DateTime.UtcNow;

            _context.Colors.Update(color);
            await _context.SaveChangesAsync();

            return new ColorDto
            {
                Id = color.Id,
                Name = color.Name,
                StoreId = color.StoreId,
                Value = color.Value,
                CreatedAt = color.CreatedAt,
                UpdatedAt = color.UpdatedAt
            };
        }

        // ✅ Delete a color
        public async Task<bool> DeleteColorAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

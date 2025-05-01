using E_comerce.Models;
using E_comerce.DTO;
using Microsoft.EntityFrameworkCore;
using E_comerce.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Dtos;
using E_commerce.Models;
using E_comrece.DTO;

namespace E_comerce.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryService> _logger; // Declare the logger

        public CategoryService(AppDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;  // Inject the logger
        }
        // Get all categories
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Label = c.Billboard.Label,
                    StoreId = c.StoreId,
                    BillboardId = c.BillboardId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        // Get categories by StoreId
        public async Task<IEnumerable<CategoryDto>> GetCategoriesByStoreIdAsync(int storeId)
        {
            return await _context.Categories
                .Where(c => c.StoreId == storeId)
                  .Include(c => c.Billboard) // this line is critical
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Label = c.Billboard.Label, // Will now be properly populated
                    StoreId = c.StoreId,
                    BillboardId = c.BillboardId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        // Get a single category by Id
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                StoreId = category.StoreId,
                BillboardId = category.BillboardId,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        // Create a new category
        public async Task<Category> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            try
            {
                // Your category creation logic here
                var category = new Category
                {
                    Name = categoryDto.Name,
                    StoreId = categoryDto.StoreId,
                    BillboardId = categoryDto.BillboardId

                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating category: ", ex);
                throw; // Re-throw to be caught by global exception handler
            }
        }



        // Update an existing category
        public async Task<Category?> UpdateCategoryAsync(int storeId, int categoryId, CategoryUpdateDto categoryDto)
        {
            // Find the category by ID and ensure it belongs to the correct store
            var category = await _context.Categories
                                          .FirstOrDefaultAsync(c => c.Id == categoryId && c.StoreId == storeId);

            // If category is not found or does not belong to the store, return null
            if (category == null)
                return null;

            // Update the category properties
            category.Name = categoryDto.name;
            category.BillboardId = categoryDto.billboardId;
            category.UpdatedAt = DateTime.UtcNow;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return the updated category
            return category;
        }

        // Delete a category
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if a store exists (helper method)
        public async Task<bool> DoesStoreExistAsync(int storeId)
        {
            return await _context.Stores.AnyAsync(s => s.Id == storeId);
        }

        // Check if a billboard exists (helper method)
        public async Task<bool> DoesBillboardExistAsync(int billboardId)
        {
            return await _context.Billboards.AnyAsync(b => b.Id == billboardId);
        }

    
    }
}

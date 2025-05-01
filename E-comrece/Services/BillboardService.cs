using E_comerce.Models;
using E_comerce.DTO;
using Microsoft.EntityFrameworkCore;
using E_comerce.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_comerce.Services
{
    public class BillboardService
    {
        private readonly AppDbContext _context;

        public BillboardService(AppDbContext context)
        {
            _context = context;
        }

        // Get all billboards
        public async Task<IEnumerable<BillboardDto>> GetAllBillboardsAsync()
        {
            return await _context.Billboards
                .Select(b => new BillboardDto
                {
                    Id = b.Id,
                    StoreId = b.StoreId,
                    Label = b.Label,
                    ImageUrl = b.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();
        }

        // Get billboards by StoreId
        public async Task<IEnumerable<BillboardDto>> GetBillboardsByStoreIdAsync(int storeId)
        {
            return await _context.Billboards
                .Where(b => b.StoreId == storeId)
                .Select(b => new BillboardDto
                {
                    Id = b.Id,
                    StoreId = b.StoreId,
                    Label = b.Label,
                    ImageUrl = b.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();
        }

        // Get a single billboard by Id
        public async Task<BillboardDto?> GetBillboardByIdAsync(int id)
        {
            var billboard = await _context.Billboards.FindAsync(id);
            if (billboard == null) return null;

            return new BillboardDto
            {
                Id = billboard.Id,
                StoreId = billboard.StoreId,
                Label = billboard.Label,
                ImageUrl = billboard.ImageUrl,
                CreatedAt = billboard.CreatedAt,
                UpdatedAt = billboard.UpdatedAt
            };
        }

        // Create a new billboard
        public async Task<BillboardDto> CreateBillboardAsync(BillboardDto billboardDto)
        {
            // Validate inputs (additional validation can be added)
            if (string.IsNullOrWhiteSpace(billboardDto.Label) || string.IsNullOrWhiteSpace(billboardDto.ImageUrl))
            {
                throw new ArgumentException("Label and ImageUrl are required.");
            }

            // Check if the store exists
            if (!await DoesStoreExistAsync(billboardDto.StoreId))
            {
                throw new InvalidOperationException($"Store with ID {billboardDto.StoreId} does not exist.");
            }

            // Create the new billboard entity
            var newBillboard = new Billboard
            {
                StoreId = billboardDto.StoreId,
                Label = billboardDto.Label,
                ImageUrl = billboardDto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the new billboard to the database
            _context.Billboards.Add(newBillboard);
            await _context.SaveChangesAsync();

            // Return the DTO with the new Billboard Id
            billboardDto.Id = newBillboard.Id;
            return billboardDto;
        }
        public async Task<Billboard?> UpdateBillboardAsync(int billboardId, BillboardDto billboardDto)
        {
            var billboard = await _context.Billboards.FindAsync(billboardId);
            if (billboard == null)
                return null;

            billboard.Label = billboardDto.Label;
            billboard.ImageUrl = billboardDto.ImageUrl;

            await _context.SaveChangesAsync();
            return billboard;
        }
        // Delete a billboard
        public async Task<bool> DeleteBillboardAsync(int id)
        {
            var billboard = await _context.Billboards.FindAsync(id);
            if (billboard == null) return false;

            _context.Billboards.Remove(billboard);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if a store exists (helper method)
        public async Task<bool> DoesStoreExistAsync(int storeId)
        {
            return await _context.Stores.AnyAsync(s => s.Id == storeId);  // Assuming you have a Stores table with Id field
        }
    }
}

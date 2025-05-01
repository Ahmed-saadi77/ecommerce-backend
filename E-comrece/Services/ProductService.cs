using E_comerce.Data;
using E_commerce.DTOs;
using E_comerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly IImageStorageService _imageStorageService;

        public ProductService(AppDbContext context, IImageStorageService imageStorageService)
        {
            _context = context;
            _imageStorageService = imageStorageService;
        }

        public async Task<List<ProductDto>> GetFilteredProductsAsync(
        int? categoryId = null,
        int? colorId = null,
        int? sizeId = null,
        bool? isFeatured = null)
        {
            var query = _context.Products.AsQueryable();

            // Apply filtering by storeId
          

            // Apply optional filters
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (colorId.HasValue)
                query = query.Where(p => p.ColorId == colorId);

            if (sizeId.HasValue)
                query = query.Where(p => p.SizeId == sizeId);

            if (isFeatured.HasValue)
                query = query.Where(p => p.isFeatured == isFeatured);

            // Include related data and fetch results
            var products = await query
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }


        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToProductDto(product);
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<List<ProductDto>> GetProductsByStoreIdAsync(int storeId)
        {
            var products = await _context.Products
                .Where(p => p.StoreId == storeId)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<List<ProductDto>> GetFeaturedProductsAsync(int storeId)
        {
            var products = await _context.Products
                .Where(p => p.StoreId == storeId && p.isFeatured)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<List<ProductDto>> GetArchivedProductsAsync(int storeId)
        {
            var products = await _context.Products
                .Where(p => p.StoreId == storeId && p.isArchived)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Include(p => p.Color)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto, List<IFormFile> imageFiles = null, List<string> imageUrls = null)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                isFeatured = productDto.IsFeatured,
                isArchived = productDto.IsArchived,
                StoreId = productDto.StoreId,
                CategoryId = productDto.CategoryId,
                SizeId = productDto.SizeId,
                ColorId = productDto.ColorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 🔹 Extract URLs from images array if provided
            if (productDto.Images != null && productDto.Images.Count > 0)
            {
                var extractedUrls = productDto.Images.Select(img => img.Url).ToList();
                await AddImagesToProductFromUrls(product.Id, extractedUrls);
            }

            return await GetProductByIdAsync(product.Id);
        }


        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto productDto, List<IFormFile> imageFiles = null, List<string> imageUrls = null, List<int> imagesToRemove = null)
        {
            // Fetch the product with its images from the database
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            // Update the basic product details
            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.isFeatured = productDto.IsFeatured;
            product.isArchived = productDto.IsArchived;
            product.CategoryId = productDto.CategoryId;
            product.SizeId = productDto.SizeId;
            product.ColorId = productDto.ColorId;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();  // Save basic product changes

            // Handle image removal
            if (imagesToRemove != null && imagesToRemove.Count > 0)
            {
                // Fetch the images that are marked for deletion
                var imagesToDelete = product.Images.Where(img => imagesToRemove.Contains(img.Id)).ToList();

                // Delete images from storage
                foreach (var image in imagesToDelete)
                {
                    await _imageStorageService.DeleteImage(image.Url);  // Ensure this deletes the file from storage
                }

                // Remove images from the database
                _context.Images.RemoveRange(imagesToDelete);
            }

            // Add new image URLs if provided
            if (imageUrls != null && imageUrls.Count > 0)
            {
                await AddImagesToProductFromUrls(product.Id, imageUrls);
            }

            

            // Save the changes after image modifications
            await _context.SaveChangesAsync();

            // Return the updated product as a DTO
            return await GetProductByIdAsync(product.Id);
        }






        public async Task AddImagesToProductFromUrls(int productId, List<string> imageUrls)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) throw new ArgumentException("Product not found");

            Console.WriteLine($"Adding images to product ID: {productId}");

            // Collect all new images
            var newImages = new List<Image>();
            foreach (var url in imageUrls)
            {
                Console.WriteLine($"Processing Image URL: {url}");
                var imageUrl = await _imageStorageService.SaveImageFromUrl(url);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    Console.WriteLine($"Saving Image: {imageUrl}");
                    newImages.Add(new Image { Url = imageUrl, ProductId = product.Id });
                }
            }

            // Batch insert images
            if (newImages.Any())
            {
                _context.Images.AddRange(newImages);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        private static ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                IsFeatured = product.isFeatured,
                IsArchived = product.isArchived,
                StoreId = product.StoreId,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "No Category",
                SizeId = product.SizeId,
                SizeName = product.Size?.Name ?? "No Size",
                ColorId = product.ColorId,
                ColorName = product.Color?.Value ?? "No Color",
                Images = product.Images?.Select(i => new ImageDto { Id = i.Id, Url = i.Url, ProductId = i.ProductId }).ToList() ?? new List<ImageDto>(),
                // ✅ Fix: Convert 0001-01-01T00:00:00 to null
                CreatedAt = (DateTime)(product.CreatedAt == DateTime.MinValue ? null : product.CreatedAt),
                UpdatedAt = (DateTime)(product.UpdatedAt == DateTime.MinValue ? null : product.UpdatedAt)
            };
        }
    }
}
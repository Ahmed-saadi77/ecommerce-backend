using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "uploads";  // Folder where images will be stored

        public LocalImageStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Save image from file upload (IFormFile)
        public async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            var uploadsPath = Path.Combine(_env.WebRootPath, ImageFolder);

            try
            {
                // Create directory if not exists
                Directory.CreateDirectory(uploadsPath);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{ImageFolder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving image", ex);
            }
        }

        // Delete image based on URL
        public async Task DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);  // Delete the file from the disk
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deleting image", ex);
            }
        }

        // Save image from URL
        public async Task<string> SaveImageFromUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Invalid image URL.");

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUrl)}";
            var uploadsPath = Path.Combine(_env.WebRootPath, ImageFolder);

            try
            {
                Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var client = new HttpClient())
                {
                    // Download image bytes from the URL
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);

                    // Save the image bytes to the file system
                    await File.WriteAllBytesAsync(filePath, imageBytes);
                }

                return $"/{ImageFolder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error downloading image from URL", ex);
            }
        }

        // Save image from file upload (asynchronously)
        public async Task<string?> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            var uploadsPath = Path.Combine(_env.WebRootPath, ImageFolder);

            try
            {
                Directory.CreateDirectory(uploadsPath);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save the file asynchronously
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{ImageFolder}/{uniqueFileName}";  // Return relative URL
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving image asynchronously", ex);
            }
        }

        // Save image from URL (asynchronously)
        public async Task<string> SaveImageFromUrlAsync(string imageUrl)
        {
            return await SaveImageFromUrl(imageUrl);  // Directly return the result of SaveImageFromUrl method
        }
    }
}

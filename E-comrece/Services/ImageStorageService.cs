using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ImageStorageService(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            var backendUrl = _configuration["BackendBaseUrl"];
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

            Directory.CreateDirectory(uploadsPath);

            // ✅ Generate a unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{backendUrl}/uploads/{uniqueFileName}"; // Return the full URL
        }

        public async Task<string> SaveImageFromUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Invalid image URL.");

            var backendUrl = _configuration["BackendBaseUrl"];
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

            Directory.CreateDirectory(uploadsPath);

            // ✅ Generate a unique filename
            var extension = Path.GetExtension(imageUrl);
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg"; // default fallback if no extension

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(filePath, imageBytes);
            }

            return $"{backendUrl}/uploads/{uniqueFileName}";
        }

        public async Task DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<string> SaveImageFromUrlAsync(string imageUrl)
        {
            return await SaveImageFromUrl(imageUrl);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveImage(IFormFile file);  // Save image from file (file upload)
        Task DeleteImage(string imageUrl);       // Delete image by URL
        Task<string> SaveImageFromUrl(string imageUrl);  // Save image from a URL
        Task<string> SaveImageFromUrlAsync(string imageUrl);  // Save image from URL asynchronously
    }
}


using E_commerce.Models;

using System.ComponentModel.DataAnnotations;
using E_comrece.Models;

namespace E_comerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool isFeatured { get; set; }
        public bool isArchived { get; set; }

        [Required]
        public int StoreId { get; set; }

        // Navigation property for Store
        public Store? Store { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int SizeId { get; set; }
        public Size? Size { get; set; }

        public int ColorId { get; set; }
        public Color? Color { get; set; }

        // Navigation property for Images (One-to-Many relationship)
        public List<Image> Images { get; set; } = new List<Image>();
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

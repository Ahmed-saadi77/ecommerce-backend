using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsArchived { get; set; }
        public int StoreId { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // ✅ Add This

        public int SizeId { get; set; }
        public string SizeName { get; set; } // ✅ Add This

        public int ColorId { get; set; }
        public string ColorName { get; set; } // ✅ Add This

        public List<ImageDto> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
         public DateTime UpdatedAt { get; set; }
    

    }
}
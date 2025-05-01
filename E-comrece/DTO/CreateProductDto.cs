using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs
{
    public class CreateProductDto
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsArchived { get; set; }
        public int CategoryId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public List<ImageDto> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
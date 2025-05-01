using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsArchived { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SizeId { get; set; }

        [Required]
        public int ColorId { get; set; }

        public List<ImageDto> Images { get; set; } = new();

    }
}
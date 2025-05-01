using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_comerce.Models;
using E_comrece.Models;

namespace E_commerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Store Foreign Key
        [Required]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store Store { get; set; }

        // Billboard Foreign Key
        [Required]
        public int BillboardId { get; set; }
        [ForeignKey("BillboardId")]
        public Billboard Billboard { get; set; }

        public ICollection<Product> Products { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

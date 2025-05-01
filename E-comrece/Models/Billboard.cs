using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_comerce.Models
{
    public class Billboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StoreId { get; set; } // Foreign Key

        [Required]
        public string Label { get; set; } = string.Empty;
        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Navigation property for Store
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
    }
}

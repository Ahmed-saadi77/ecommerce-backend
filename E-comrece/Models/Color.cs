using E_comerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_comerce.Models
{
    public class Color
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Store Foreign Key
        [Required]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

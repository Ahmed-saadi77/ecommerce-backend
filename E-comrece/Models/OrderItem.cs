using E_comerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_comrece.Models
{
    public class OrderItem
    {
        public int Id { get; set; }


        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; } = 1; // ✅ Add quantity support

    }
}

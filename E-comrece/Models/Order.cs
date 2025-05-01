using E_comerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_comrece.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public bool isPaid { get; set; }

        [Required]
        public int StoreId { get; set; }
        public Store? Store { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        // ✅ Proper navigation property
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

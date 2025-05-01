using System.ComponentModel.DataAnnotations.Schema;

namespace E_comerce.Models
{
    public class Image
    {
        public int Id { get; set; }
       
        public int ProductId { get; set; }
   
        public Product? Product { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}

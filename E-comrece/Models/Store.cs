using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using E_comerce.Models;
using E_commerce.Models;
using E_comrece.Models;
using Microsoft.AspNetCore.Identity;

namespace E_comerce.Models
{
    public class Store : StandardModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; } // User who owns the store

        [Required]
        public string Name { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        // ✅ One-to-Many Relationship with Billboard
        public ICollection<Billboard> Billboards { get; set; } = new List<Billboard>();

        // ✅ One-to-Many Relationship with Category
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<Size> Sizes { get; set; }

        public ICollection<Color> Colors { get; set; }
        public ICollection<Product> Products { get; set; }



    }
}

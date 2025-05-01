using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_comerce.Models;
using E_commerce.Models;
using E_comrece.Models;
using Microsoft.AspNetCore.Identity;

namespace E_comerce.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Billboard> Billboards { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Store>()
     .HasOne(s => s.User)
     .WithMany() // You can use .WithMany("Stores") if you add a collection navigation to IdentityUser
     .HasForeignKey(s => s.UserId)
     .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Billboard>()
                .HasOne(b => b.Store)
                .WithMany(s => s.Billboards)
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Store)
                .WithMany(s => s.Categories)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Billboard)
                .WithMany()
                .HasForeignKey(c => c.BillboardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Size>()
                .HasOne(s => s.Store)
                .WithMany(st => st.Sizes)
                .HasForeignKey(s => s.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Color>()
                .HasOne(c => c.Store)
                .WithMany(st => st.Colors)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Store)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Size)
                    .WithMany()
                    .HasForeignKey(p => p.SizeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Color)
                    .WithMany()
                    .HasForeignKey(p => p.ColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Images)
                    .WithOne(i => i.Product)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

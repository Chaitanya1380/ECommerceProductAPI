using Microsoft.EntityFrameworkCore;
using ECommerceProjectAPI.Models;

namespace ECommerceProjectAPI.Data
{
    public class ProductDBContext : DbContext
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options):base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)              // Product has one Category
            .WithMany(c => c.Products)            // Category has many Products
            .HasForeignKey(p => p.CategoryId)     // FK in Product
            .OnDelete(DeleteBehavior.Cascade);    // Delete Products when Category is deleted

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Books" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", CategoryId = 1 },
                new Product { Id = 2, Name = "Smartphone", CategoryId = 1 },
                new Product { Id = 3, Name = "T-Shirt", CategoryId = 2 },
                new Product { Id = 4, Name = "Novel", CategoryId = 3 }
            );
        }

    }
}

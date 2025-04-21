using Drinkbox.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Models
{
    public class VendomatContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coin> Coins { get; set; }

        public VendomatContext(DbContextOptions<VendomatContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=Vendomax;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<Coin>()
                .Ignore(c => c.Value);
        }
    }
}

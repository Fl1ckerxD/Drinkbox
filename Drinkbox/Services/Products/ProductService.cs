using Drinkbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly DrinkboxContext _context;
        public ProductService(DrinkboxContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Product>> GetAllAsync() => await _context.Products.ToListAsync();

        public async Task<ICollection<Product>> GetByBrandAsync(int? brandId)
        {
            var products = await GetAllAsync();

            if (brandId.HasValue)
                products = products.Where(p => p.BrandId == brandId).ToList();

            return products;
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public ICollection<Product> GetByMaxPrice(ICollection<Product> products, int maxPrice)
        {
            return products.Where(p => p.Price <= maxPrice).ToList();
        }
    }
}

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

        public async Task<ICollection<Product>> GetAllProductsAsync() => await _context.Products.ToListAsync();

        public async Task<ICollection<Product>> GetProductsByBrandAsync(int? brandId)
        {
            var products = await GetAllProductsAsync();

            if (brandId.HasValue)
                products = products.Where(p => p.BrandId == brandId).ToList();

            return products;
        }

        public ICollection<Product> GetProductsByMaxPrice(ICollection<Product> products, int maxPrice)
        {
            return products.Where(p => p.Price <= maxPrice).ToList();
        }
    }
}

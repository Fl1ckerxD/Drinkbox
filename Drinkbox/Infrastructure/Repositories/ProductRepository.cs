using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(VendomatContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products
                .Where(p => p.BrandId == brandId)
                .ToListAsync();
        }
    }
}

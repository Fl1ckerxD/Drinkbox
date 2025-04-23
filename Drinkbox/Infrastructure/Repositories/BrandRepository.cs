using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(VendomatContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Brand>> GetActiveBrandsAsync()
        {
            return await _context.Brands
                .Where(b => b.Products.Any(p => p.IsActive))
                .ToListAsync();
        }

        public async Task<Brand?> GetBrandWithProductsAsync(int id)
        {
            return await _context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.BrandId == id);
        }
    }
}

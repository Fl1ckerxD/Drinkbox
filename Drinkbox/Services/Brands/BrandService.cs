using Drinkbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly DrinkboxContext _context;
        public BrandService(DrinkboxContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Brand>> GetAllBrandsAsync() => await _context.Brands.ToListAsync();
    }
}

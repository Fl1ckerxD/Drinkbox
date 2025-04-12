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

        public async Task<ICollection<Product>> GetAllProductsAsyc() => await _context.Products.ToListAsync();
    }
}

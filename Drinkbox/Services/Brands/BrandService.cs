using Drinkbox.Models;
using Drinkbox.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Services.Brands
{
    /// <summary>
    /// Сервис для работы с брендами.
    /// Реализует интерфейс IBrandService для выполнения операций с брендами.
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly VendomatContext _context;
        public BrandService(VendomatContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Brand>> GetAllBrandsAsync() => await _context.Brands.ToListAsync();
    }
}

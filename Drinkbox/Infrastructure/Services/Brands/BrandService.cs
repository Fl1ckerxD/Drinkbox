using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Drinkbox.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Services.Brands
{
    /// <summary>
    /// Сервис для работы с брендами.
    /// Реализует интерфейс IBrandService для выполнения операций с брендами.
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IRepository<Brand> _repo;
        public BrandService(IRepository<Brand> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync() => await _repo.GetAllAsync();
    }
}

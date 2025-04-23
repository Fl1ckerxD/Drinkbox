using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Repositories;

namespace Drinkbox.Infrastructure.Services.Brands
{
    /// <summary>
    /// Сервис для работы с брендами.
    /// Реализует интерфейс IBrandService для выполнения операций с брендами.
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _uow;
        public BrandService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync() => await _uow.Brands.GetAllAsync();
    }
}

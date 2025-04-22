using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Services.Brands
{
    public interface IBrandService
    {
        /// <summary>
        /// Асинхронно получает список всех брендов из базы данных.
        /// </summary>
        /// <returns>Коллекция всех брендов.</returns>
        Task<ICollection<Brand>> GetAllBrandsAsync();
    }
}

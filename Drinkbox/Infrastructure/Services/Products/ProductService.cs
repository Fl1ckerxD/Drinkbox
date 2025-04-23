using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Drinkbox.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        /// <summary>
        /// Сервис для работы с продуктами.
        /// Реализует интерфейс IProductService для управления продуктами.
        /// </summary>
        private readonly IUnitOfWork _uow;
        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _uow.Products.GetAllAsync();

        public async Task<IEnumerable<Product>> GetByBrandAsync(int? brandId)
        {
            var products = await GetAllAsync();

            if (brandId.HasValue)
                products = products.Where(p => p.BrandId == brandId).ToList();

            return products;
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _uow.Products.GetByIdAsync(productId);
        }

        public ICollection<Product> GetByMaxPrice(IEnumerable<Product> products, int maxPrice)
        {
            return products.Where(p => p.Price <= maxPrice).ToList();
        }
    }
}

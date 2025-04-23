using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    }
}

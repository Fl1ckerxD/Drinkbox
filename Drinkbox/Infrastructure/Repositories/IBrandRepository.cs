using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Repositories
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<IEnumerable<Brand>> GetActiveBrandsAsync();
        Task<Brand?> GetBrandWithProductsAsync(int id);
    }
}

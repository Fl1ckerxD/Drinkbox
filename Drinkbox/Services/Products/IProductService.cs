using Drinkbox.Models;

namespace Drinkbox.Services.Products
{
    public interface IProductService
    {
        Task<Product> GetByIdAsync(int productId);
        Task<ICollection<Product>> GetAllAsync();
        Task<ICollection<Product>> GetByBrandAsync(int? brandId);
        ICollection<Product> GetByMaxPrice(ICollection<Product> products, int maxPrice);
    }
}

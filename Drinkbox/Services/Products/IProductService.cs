using Drinkbox.Models;

namespace Drinkbox.Services.Products
{
    public interface IProductService
    {
        Task<ICollection<Product>> GetAllProductsAsync();
        Task<ICollection<Product>> GetProductsByBrandAsync(int? brandId);
        ICollection<Product> GetProductsByMaxPrice(ICollection<Product> products, int maxPrice);
    }
}

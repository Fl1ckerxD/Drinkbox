using Drinkbox.Models;

namespace Drinkbox.Services.Products
{
    public interface IProductService
    {
        Task<ICollection<Product>> GetAllProductsAsyc();
    }
}

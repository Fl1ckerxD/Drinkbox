using Drinkbox.Models;

namespace Drinkbox.Services.Brands
{
    public interface IBrandService
    {
        Task<ICollection<Brand>> GetAllBrandsAsync();
    }
}

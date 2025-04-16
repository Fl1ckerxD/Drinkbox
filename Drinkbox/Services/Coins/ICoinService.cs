using Drinkbox.Models;

namespace Drinkbox.Services.Coins
{
    public interface ICoinService
    {
        Task<Coin> GetByIdAsync(int coinId);
        Task<ICollection<Coin>> GetAllAsync();
    }
}

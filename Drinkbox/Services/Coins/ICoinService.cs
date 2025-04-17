using Drinkbox.Models;

namespace Drinkbox.Services.Coins
{
    public interface ICoinService
    {
        Task<ICollection<Coin>> GetAllAsync();
        Task SaveCoinsAsync(List<Coin> coins);
    }
}

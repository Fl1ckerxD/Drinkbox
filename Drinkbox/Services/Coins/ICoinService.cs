using Drinkbox.Models;

namespace Drinkbox.Services.Coins
{
    public interface ICoinService
    {
        Task<ICollection<Coin>> GetAllAsync();
        Task SaveCoinsAsync(List<CoinInput> coins);
        Task<Dictionary<int, int>> CalculateChange(int changeAmount);
        Task UpdateQuantityCoins(Dictionary<int, int> coinsToRemove);
    }
}

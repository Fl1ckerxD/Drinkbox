using Drinkbox.Core.DTOs;
using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Services.Coins
{
    /// <summary>
    /// Сервис для работы с монетами.
    /// Реализует интерфейс ICoinService для управления монетами.
    /// </summary>
    public class CoinService : ICoinService
    {
        private readonly VendomatContext _context;
        public CoinService(VendomatContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Coin>> GetAllAsync() => await _context.Coins.ToListAsync();

        public async Task SaveCoinsAsync(List<CoinInput> userCoins)
        {
            var coins = await GetAllAsync();

            foreach (var coin in coins)
            {
                var userCoin = userCoins.FirstOrDefault(u => u.CoinId == coin.CoinId);
                if (userCoin != null)
                    coin.Quantity += userCoin.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityCoins(Dictionary<int, int> coinsToRemove)
        {
            foreach (var (denomination, quantity) in coinsToRemove)
            {
                var coins = await GetAllAsync();
                var coin = coins.FirstOrDefault(x => x.Value == denomination);

                if (coin != null)
                    coin.Quantity = Math.Max(0, coin.Quantity - quantity);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<int, int>> CalculateChange(int changeAmount)
        {
            var coins = await GetAllAsync();
            var denominations = new[] { 10, 5, 2, 1 };
            var change = new Dictionary<int, int>();

            foreach (var denom in denominations)
            {
                int quantity = coins.FirstOrDefault(x => x.Value == denom).Quantity;
                if (changeAmount >= denom && quantity > 0)
                {
                    int maxPossible = Math.Min(quantity, changeAmount / denom);
                    if (maxPossible > 0)
                    {
                        change.Add(denom, maxPossible);
                        changeAmount -= maxPossible * denom;
                    }
                }
            }

            if (changeAmount > 0)
                throw new Exception($"Извините, в данный момент мы не можем продать вам товар по причине того, что автомат не может выдать вам нужную сдачу");

            return change;
        }
    }
}

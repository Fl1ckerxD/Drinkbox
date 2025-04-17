using Drinkbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Services.Coins
{
    public class CoinService : ICoinService
    {
        private readonly DrinkboxContext _context;
        public CoinService(DrinkboxContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Coin>> GetAllAsync() => await _context.Coins.ToListAsync();

        public async Task SaveCoinsAsync(List<Coin> userCoins)
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
    }
}

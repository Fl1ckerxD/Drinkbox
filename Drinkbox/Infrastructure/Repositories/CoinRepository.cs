using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Repositories
{
    public class CoinRepository : Repository<Coin>, ICoinRepository
    {
        public CoinRepository(VendomatContext context) : base(context)
        {
        }

        public async Task<int> GetTotalMoneyAsync()
        {
            return await _context.Coins
                .SumAsync(c => c.Value.GetValueOrDefault() * c.Quantity);
        }

        public async Task UpdateQuantityAsync(int coinId, int quantity)
        {
            var coin = await GetByIdAsync(coinId);
            if (coin != null)
            {
                coin.Quantity = quantity;
                await UpdateAsync(coin);
            }
        }
    }
}

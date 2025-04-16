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

        public async Task<Coin> GetByIdAsync(int coinId)
        {
            return await _context.Coins.FirstOrDefaultAsync(c => c.CoinId == coinId);
        }
    }
}

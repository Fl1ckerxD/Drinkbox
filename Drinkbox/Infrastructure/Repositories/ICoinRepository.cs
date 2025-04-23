using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Repositories
{
    public interface ICoinRepository : IRepository<Coin>
    {
        Task UpdateQuantityAsync(int coinId, int quantity);
        Task<int> GetTotalMoneyAsync();
    }
}

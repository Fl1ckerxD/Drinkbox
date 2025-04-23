using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end);
        Task<Order?> GetOrderWithItemsAsync(int id);
        void AddOrderItem(OrderItem item);
    }
}

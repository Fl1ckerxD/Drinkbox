using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(VendomatContext context) : base(context)
        {
        }

        public void AddOrderItem(OrderItem item)
        {
            _context.OrderItems.Add(item);
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();
        }
    }
}

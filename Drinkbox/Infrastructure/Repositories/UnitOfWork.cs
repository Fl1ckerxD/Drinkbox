
using AspNetCoreGeneratedDocument;
using Drinkbox.Infrastructure.Data;

namespace Drinkbox.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VendomatContext _context;
        private bool _disposed;

        public UnitOfWork(VendomatContext context)
        {
            _context = context;
            Brands = new BrandRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Coins = new CoinRepository(_context);
        }

        public IBrandRepository Brands { get; }
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public ICoinRepository Coins { get; }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

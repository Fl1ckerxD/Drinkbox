namespace Drinkbox.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBrandRepository Brands { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders {  get; }
        ICoinRepository Coins { get; }
        Task<int> CommitAsync();
    }
}

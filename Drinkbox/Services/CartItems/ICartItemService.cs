using Drinkbox.Models;

namespace Drinkbox.Services.CartItems
{
    public interface ICartItemService
    {
        List<CartItem> CartItems { get; }
        void AddToCart(Product product, int quantity = 1);
        void RemoveFromCart(Product product);
        void LoadCart();
        void SaveCart();
        Task CompleteOrder();
    }
}
